using AutoMapper;
using Azure;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Azure.AI.ContentSafety;
using Azure.Core;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PerspectiveService : BaseService<PerspectiveService>, IPerspectiveService
    {
        private readonly ContentSafetyClient _client;
        //private string endpoint = "https://commentmoderationservice.openai.azure.com/";
        //private string endpoint = "https://commentmoderationservice.services.ai.azure.com/";
        //private string endpoint = "https://commentmoderationservice.cognitiveservices.azure.com/";

        //public async Task<bool> IsCommentToxicAsync(string comment)
        //{
        //    var payload = new
        //    {
        //        comment = new { text = comment },
        //        languages = new[] { "en" },
        //        requestedAttributes = new
        //        {
        //            TOXICITY = new { }
        //        }
        //    };

        //    var jsonPayload = JsonConvert.SerializeObject(payload);
        //    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        //    var response = await _httpClient.PostAsync($"{ApiUrl}?key={ApiKey}", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        dynamic result = JsonConvert.DeserializeObject(responseContent);

        //        // Truy xuất điểm "TOXICITY" từ kết quả
        //        float score = result.attributeScores.TOXICITY.summaryScore.value;
        //        return score > 0.8; // Nếu điểm trên 0.8, bình luận được coi là độc hại
        //    }

        //    throw new Exception("Error analyzing comment.");
        //}

        public PerspectiveService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<PerspectiveService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            // Lấy API key và endpoint từ configuration
            var apiKey = configuration["PerspectiveAPI:ApiKey"];
            var endpoint = configuration["PerspectiveAPI:Endpoint"];

            // Kiểm tra null
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("Content Safety API Key is not configured");

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException("Content Safety Endpoint is not configured");

            // Tạo credentials
            var credentials = new AzureKeyCredential(apiKey);
            _client = new ContentSafetyClient(new Uri(endpoint), credentials);
        }

        //public bool IsContentToxic(string comment)
        //{
        // Phân tích cảm xúc hoặc phát hiện nội dung độc hại
        //DocumentSentiment sentiment = _client.AnalyzeSentiment(comment);

        //// Log thông tin phân tích (tuỳ chọn)
        //Console.WriteLine($"Sentiment: {sentiment.Sentiment}");

        //// Kiểm tra nếu cảm xúc là tiêu cực
        //return sentiment.Sentiment == TextSentiment.Negative;
        //}

        public async Task<bool> IsContentAppropriate(string content)
        {
            // Kiểm duyệt văn bản
            var analyzeOptions = new AnalyzeTextOptions(content)
            {
                Categories = { TextCategory.Hate, TextCategory.SelfHarm, TextCategory.Violence }
            };

            var response = await _client.AnalyzeTextAsync(analyzeOptions);

            // Kiểm tra kết quả phân tích
            foreach (var categoryResult in response.Value.CategoriesAnalysis)
            {
                // Nếu có bất kỳ danh mục nào đánh giá nội dung không phù hợp
                if (categoryResult.Severity >= 2) // Tùy thuộc vào ngưỡng bạn muốn kiểm tra, có thể chỉnh lên 3
                {
                    return false; // Nội dung không phù hợp
                }
            }

            return true; // Nội dung phù hợp
        }
    }
}