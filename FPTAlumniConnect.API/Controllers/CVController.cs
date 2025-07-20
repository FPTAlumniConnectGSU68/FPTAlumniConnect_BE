using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.API.Services.Implements;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class CVController : BaseController<CVController>
    {
        private readonly ICVService _cVService;

        public CVController(ILogger<CVController> logger, ICVService cVService) : base(logger)
        {
            _cVService = cVService;
        }

        [HttpGet(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCVById(int id)
        {
            try
            {
                var response = await _cVService.GetCVById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.CV.CVUserEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCVByUserId(int id)
        {
            try
            {
                var response = await _cVService.GetCVByUserId(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCV([FromBody] CVInfo request)
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
                var id = await _cVService.CreateNewCV(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllCV([FromQuery] CVFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _cVService.ViewAllCV(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCVInfo(int id, [FromBody] CVInfo request)
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
                var isSuccessful = await _cVService.UpdateCVInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        //[HttpPost("share")]
        //public async Task<IActionResult> ShareCvByEmail([FromBody] ShareCvRequest request)
        //{
        //    await _cVService.ShareCvByEmailAsync(request);
        //    return Ok(new { status = "success", message = "CV shared successfully." });
        //}

        [HttpPatch("{cvId}/toggle-job-looking")]
        public async Task<IActionResult> ToggleJobLooking(int cvId)
        {
            var result = await _cVService.ToggleIsLookingForJobAsync(cvId);
            return Ok(new { status = "success", isLooking = result });
        }

        //[HttpGet("{cvId}/export")]
        //public async Task<IActionResult> ExportCvToPdf(int cvId)
        //{
        //    var pdfBytes = await _cVService.ExportCvToPdfAsync(cvId);
        //    return File(pdfBytes, "application/pdf", "cv.pdf");
        //}

    }
}
