using System.Text.Json;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class MajorCodeController : BaseController<MajorCodeController>
    {
        private readonly IMajorCodeService _majorCodeService;

        public MajorCodeController(ILogger<MajorCodeController> logger, IMajorCodeService majorCodeService) : base(logger)
        {
            _majorCodeService = majorCodeService;
        }

        [HttpGet(ApiEndPointConstant.MajorCode.MajorCodeEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMajorCodeById(int id)
        {
            try
            {
                var response = await _majorCodeService.GetMajorCodeById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when fetching major code");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch major code");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.MajorCode.MajorCodesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewMajorCode([FromBody] MajorCodeInfo request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors
                });
            }

            try
            {
                var id = await _majorCodeService.CreateNewMajorCode(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when creating major code");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create major code");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.MajorCode.MajorCodesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllMajorCodes([FromQuery] MajorCodeFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _majorCodeService.ViewAllMajorCode(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when fetching all major codes");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch major code");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.MajorCode.MajorCodeEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMajorCodeInfo(int id, [FromBody] MajorCodeInfo request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors
                });
            }

            try
            {
                var isSuccessful = await _majorCodeService.UpdateMajorCodeInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when updating major code");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update major code");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.MajorCode.NamesEndpoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMajorNames()
        {
            try
            {
                var response = await _majorCodeService.GetAllMajorNames();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when fetching major names");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch major name");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.MajorCode.CountEndpoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(int), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountAllMajorCodes()
        {
            try
            {
                int response = await _majorCodeService.CountMajorCodesAsync();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when counting major codes");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch major code count");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}