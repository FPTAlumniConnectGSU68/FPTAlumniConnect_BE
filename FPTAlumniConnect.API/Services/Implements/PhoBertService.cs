using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
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
            IConfiguration configuration) // Thêm IConfiguration
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _apiToken = configuration["HuggingFace:ApiToken"]
                        ?? Environment.GetEnvironmentVariable("HUGGINGFACE_API_TOKEN")
                        ?? throw new ArgumentNullException("HuggingFace:ApiToken", "API token is not configured.");
            _httpClient = new HttpClient();
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
                var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var embeddingList = JsonSerializer.Deserialize<List<double[]>>(responseBody);
                return embeddingList?.FirstOrDefault() ?? throw new Exception("Failed to parse embedding.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating embedding: {ex.Message}", ex);
            }
        }

        private double CalculateCosineSimilarity(double[] vecA, double[] vecB)
        {
            double dotProduct = 0.0, magnitudeA = 0.0, magnitudeB = 0.0;
            for (int i = 0; i < vecA.Length; i++)
            {
                dotProduct += vecA[i] * vecB[i];
                magnitudeA += Math.Pow(vecA[i], 2);
                magnitudeB += Math.Pow(vecB[i], 2);
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            return dotProduct / (magnitudeA * magnitudeB);
        }

        private double CalculateScore(Cv cv, JobPostResponse job)
        {
            double score = 0.0;

            double locationScore = cv.City?.Equals(job.Location, StringComparison.OrdinalIgnoreCase) == true ? 1.0 : 0.5;
            score += locationScore;

            double salaryScore = (cv.MinSalary <= job.MaxSalary && cv.MaxSalary >= job.MinSalary) ? 1.0 : 0.5;
            score += salaryScore;

            double languageScore = cv.Language?.Equals(job.Requirements, StringComparison.OrdinalIgnoreCase) == true ? 1.0 : 0.5;
            score += languageScore;

            double majorScore = (cv.User?.MajorId == job.MajorId) ? 1.0 : 0.0;
            score += majorScore;

            return score / 4.0; // Cập nhật số lượng yếu tố (4 thay vì 5 do thiếu skillScore)
        }

        public async Task<List<Cv>> RecommendCVForJobPostAsync(int jobPostId)
        {
            var jobPost = await _jobPostService.GetJobPostById(jobPostId);
            if (jobPost == null) return new List<Cv>();

            var cvs = await _unitOfWork.GetRepository<Cv>().GetAllAsync();

            return cvs.Select(cv => new { Cv = cv, Score = CalculateScore(cv, jobPost) })
                      .OrderByDescending(x => x.Score)
                      .Select(x => x.Cv)
                      .ToList();
        }
    }
}