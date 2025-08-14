using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace FPTAlumniConnect.API.Services.Implements
{
    public class PhoBertService : BaseService<PhoBertService>, IPhoBertService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly ICVService _cvService;
        private readonly IJobPostService _jobPostService;

        public PhoBertService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<PhoBertService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICVService cvService,
            IJobPostService jobPostService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _apiToken = configuration["HuggingFace:ApiToken"]
                        ?? Environment.GetEnvironmentVariable("HUGGINGFACE_API_TOKEN")
                        ?? throw new ArgumentNullException("HuggingFace:ApiToken", "API token is not configured.");
            _httpClient = httpClientFactory.CreateClient(nameof(PhoBertService));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
            _cvService = cvService;
            _jobPostService = jobPostService;
        }

        public async Task<double[]> GenerateEmbedding(EmbeddingRequest text)
        {
            if (string.IsNullOrWhiteSpace(text?.Inputs))
                throw new ArgumentNullException(nameof(text), "Input text cannot be null or empty.");

            var requestBody = new { inputs = text.Inputs };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // Resilience (retry, timeout, etc.) is handled by AddStandardResilienceHandler in Program.cs
                var response = await _httpClient.PostAsync("", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Handle Hugging Face "model is loading" case
                if (responseBody.Contains("\"error\"", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception($"Hugging Face API returned an error: {responseBody}");
                }

                var embeddingList = JsonSerializer.Deserialize<List<double[]>>(responseBody);
                return embeddingList?.FirstOrDefault() ?? throw new Exception("Failed to parse embedding.");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Unauthorized: Please check your Hugging Face API token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating embedding: {ex.Message}", ex);
            }
        }

        private double CalculateCosineSimilarity(double[] vecA, double[] vecB)
        {
            if (vecA.Length != vecB.Length)
                throw new ArgumentException("Vectors must be of the same length.");

            double dotProduct = 0.0, magnitudeA = 0.0, magnitudeB = 0.0;
            for (int i = 0; i < vecA.Length; i++)
            {
                dotProduct += vecA[i] * vecB[i];
                magnitudeA += Math.Pow(vecA[i], 2);
                magnitudeB += Math.Pow(vecB[i], 2);
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0) return 0.0;

            return dotProduct / (magnitudeA * magnitudeB);
        }

        private double CalculateScore(Cv cv, JobPostResponse job)
        {
            double score = 0.0;
            int factorCount = 0;

            if (!string.IsNullOrEmpty(cv.City) && !string.IsNullOrEmpty(job.Location))
            {
                score += cv.City.Equals(job.Location, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.5;
                factorCount++;
            }

            if (cv.MinSalary != 0 && cv.MaxSalary != 0 && job.MinSalary != 0 && job.MaxSalary != 0)
            {
                bool overlaps = cv.MinSalary <= job.MaxSalary && cv.MaxSalary >= job.MinSalary;
                score += overlaps ? 1.0 : 0.5;
                factorCount++;
            }

            if (!string.IsNullOrEmpty(cv.Language) && !string.IsNullOrEmpty(job.Requirements))
            {
                score += cv.Language.Equals(job.Requirements, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.5;
                factorCount++;
            }

            var cvSkills = cv.CvSkills?.Select(s => s.Skill?.Name?.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string>();

            var jobSkills = job.Skills?
                .Select(s => s.Name?.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string>();

            if (jobSkills.Count > 0)
            {
                int matchingSkills = cvSkills.Intersect(jobSkills).Count();
                double skillScore = (double)matchingSkills / jobSkills.Count;
                score += skillScore;
                factorCount++;
            }

            if (cv.MajorId.HasValue && job.MajorId.HasValue)
            {
                score += (cv.MajorId == job.MajorId) ? 1.0 : 0.0;
                factorCount++;
            }

            return factorCount > 0 ? score / factorCount : 0.0;
        }

        public async Task<IPaginate<CVResponse>> RecommendCVForJobPostAsync(int jobPostId, PagingModel pagingModel)
        {
            var jobPost = await _jobPostService.GetJobPostById(jobPostId);
            if (jobPost == null)
                throw new BadHttpRequestException($"JobPost with ID {jobPostId} not found.");

            var cvRepository = _unitOfWork.GetRepository<Cv>();

            var cvs = await cvRepository.GetListAsync(
                predicate: c =>
                    c.MajorId.HasValue &&
                    jobPost.MajorId.HasValue &&
                    c.MajorId == jobPost.MajorId &&
                    c.Status == CVStatus.Public,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major)
            );

            var scored = cvs
                .Select(cv => new { Cv = cv, Score = CalculateScore(cv, jobPost) })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();

            var total = scored.Count;
            var items = scored
                .Skip((pagingModel.page - 1) * pagingModel.size)
                .Take(pagingModel.size)
                .Select(x => _mapper.Map<CVResponse>(x.Cv))
                .ToList();

            return new Paginate<CVResponse>
            {
                Page = pagingModel.page,
                Size = pagingModel.size,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)pagingModel.size),
                Items = items
            };
        }

    }
}