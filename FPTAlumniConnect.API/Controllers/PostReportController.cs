using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PostReportController : BaseController<PostReportController>
    {
        private readonly IPostReportService _postReportService;

        public PostReportController(ILogger<PostReportController> logger, IPostReportService postReportService) : base(logger)
        {
            _postReportService = postReportService;
        }

        [HttpGet(ApiEndPointConstant.PostReport.PostReportEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReportById(int id)
        {
            try
            {
                var response = await _postReportService.GetReportById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch report");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.PostReport.PostReportsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewReport([FromBody] PostReportInfo request)
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
                var id = await _postReportService.CreateNewReport(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create report");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.PostReport.PostReportsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllReport([FromQuery] PostReportFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _postReportService.ViewAllReport(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch report");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.PostReport.PostReportEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReportInfo(int id, [FromBody] PostReportInfo request)
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
                var isSuccessful = await _postReportService.UpdateReportInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update report");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
