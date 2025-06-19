using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.EducationHistory;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class EducationHistoryController : BaseController<EducationHistoryController>
    {
        private readonly IEducationHistoryService _educationHistoryService;

        public EducationHistoryController(ILogger<EducationHistoryController> logger, IEducationHistoryService educationHistoryService) : base(logger)
        {
            _educationHistoryService = educationHistoryService;
        }

        [HttpGet(ApiEndPointConstant.EducationHistory.EducationHistoryEndPoint)]
        [ProducesResponseType(typeof(GetEducationHistoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEducationHistory(int id)
        {
            var response = await _educationHistoryService.GetEducationHistoryById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.EducationHistory.EducationHistoriesEndPoint)]
        [ProducesResponseType(typeof(GetEducationHistoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllEducationHistory([FromQuery] EducationHistoryFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _educationHistoryService.ViewAllEducationHistory(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.EducationHistory.EducationHistoriesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEducationHistory([FromBody] EducationHistoryInfo request)
        {
            var id = await _educationHistoryService.CreateNewEducationHistory(request);
            return CreatedAtAction(nameof(GetEducationHistory), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.EducationHistory.EducationHistoryEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEducationHistory(int id, [FromBody] EducationHistoryInfo request)
        {
            var isSuccessful = await _educationHistoryService.UpdateEducationHistory(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}