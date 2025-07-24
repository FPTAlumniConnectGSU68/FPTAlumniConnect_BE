using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Payload.CV;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PhoBertService : BaseService<PhoBertService>, IPhoBertService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken = "";
        private readonly ICVService _cvService;
        private readonly IJobPostService _jobPostService;
        public PhoBertService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<PhoBertService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICVService cvService,
            IJobPostService jobPostService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
            _cvService = cvService;
            _jobPostService = jobPostService;
        }

        public async Task<double[]> GenerateEmbedding(EmbeddingRequest text)
        {
            var requestBody = new { inputs = text };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // Sử dụng đúng endpoint của mô hình
                var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2", content);
                response.EnsureSuccessStatusCode(); // Kiểm tra thành công của response

                // Đọc và xử lý phản hồi từ API
                var responseBody = await response.Content.ReadAsStringAsync();
                var embedding = JsonSerializer.Deserialize<double[]>(responseBody);

                return embedding ?? throw new Exception("Failed to generate embedding.");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                throw new Exception($"Error generating embedding: {ex.Message}");
            }
        }


        //public async Task<int?> FindBestMatchingCV(EmbeddingRequest jobDescription)
        //{
        //    var jobEmbedding = await GenerateEmbedding(jobDescription);
        //
        //    var cvRepository = _unitOfWork.GetRepository<Cv>();
        //    var cvs = await cvRepository.GetAllAsync();
        //
        //    int? bestCvId = null;
        //    double bestScore = -1.0;
        //
        //    foreach (var cv in cvs)
        //    {
        //        if (cv.Embedding != null)
        //        {
        //            var cvEmbedding = JsonSerializer.Deserialize<double[]>(cv.Embedding);
        //            if (cvEmbedding != null)
        //            {
        //                var similarityScore = CalculateCosineSimilarity(jobEmbedding, cvEmbedding);
        //                if (similarityScore > bestScore)
        //                {
        //                    bestScore = similarityScore;
        //                    bestCvId = cv.Id;
        //                }
        //            }
        //        }
        //    }
        //
        //    return bestCvId;
        //}

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

            double locationScore = cv.City.Equals(job.Location, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.5;
            score += locationScore;

            double salaryScore = (cv.MinSalary <= job.MaxSalary && cv.MaxSalary >= job.MinSalary) ? 1.0 : 0.5;
            score += salaryScore;

            double languageScore = cv.Language.Equals(job.Requirements, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.5;
            score += languageScore;

            //var matchingSkills = cv.SkillJobs.Select(s => s.Skill)
            //                                 .Intersect(job.Requirements?.Split(',') ?? new string[0], StringComparer.OrdinalIgnoreCase)
            //                                 .Count();
            //double skillScore = (double)matchingSkills / Math.Max(1, cv.SkillJobs.Count);
            //score += skillScore;

            double majorScore = (cv.User?.MajorId == job.MajorId) ? 1.0 : 0.0;
            score += majorScore;

            return score / 5.0;
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