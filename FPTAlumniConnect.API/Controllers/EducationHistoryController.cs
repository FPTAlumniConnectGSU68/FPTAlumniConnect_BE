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

        public EducationHistoryController(
            ILogger<EducationHistoryController> logger,
            IEducationHistoryService educationHistoryService)
            : base(logger)
        {
            _educationHistoryService = educationHistoryService;
        }

        [HttpGet(ApiEndPointConstant.EducationHistory.EducationHistoryEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEducationHistory(int id)
        {
            try
            {
                var response = await _educationHistoryService.GetEducationHistoryById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch education history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.EducationHistory.EducationHistoriesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllEducationHistory([FromQuery] EducationHistoryFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _educationHistoryService.ViewAllEducationHistory(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all education histories");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.EducationHistory.EducationHistoriesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEducationHistory([FromBody] EducationHistoryInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }

            try
            {
                var id = await _educationHistoryService.CreateNewEducationHistory(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create education history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.EducationHistory.EducationHistoryEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEducationHistory(int id, [FromBody] EducationHistoryInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }

            try
            {
                var isSuccessful = await _educationHistoryService.UpdateEducationHistory(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update education history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
