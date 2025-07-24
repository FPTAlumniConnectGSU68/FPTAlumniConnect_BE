using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class RecruiterInfoController : BaseController<RecruiterInfoController>
    {
        private readonly IRecruiterInfoService _recruiterService;

        public RecruiterInfoController(ILogger<RecruiterInfoController> logger, IRecruiterInfoService recruiterService)
            : base(logger)
        {
            _recruiterService = recruiterService;
        }

        [HttpGet(ApiEndPointConstant.RecruiterInfo.RecruiterInfoEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecruiterById(int id)
        {
            try
            {
                var response = await _recruiterService.GetRecruiterInfoById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch recruiter info");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.RecruiterInfo.RecruiterInfoUserEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecruiterByUserId(int id)
        {
            try
            {
                var response = await _recruiterService.GetRecruiterInfoByUserId(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch recruiter info by user");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.RecruiterInfo.RecruiterInfosEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRecruiterInfo([FromBody] RecruiterInfoInfo request)
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
                var id = await _recruiterService.CreateNewRecruiterInfo(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "RecruiterInfo created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create recruiter info");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.RecruiterInfo.RecruiterInfoEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRecruiterInfo(int id, [FromBody] RecruiterInfoInfo request)
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
                var isSuccessful = await _recruiterService.UpdateRecruiterInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update recruiter info");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.RecruiterInfo.RecruiterInfosEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllRecruiters([FromQuery] RecruiterInfoFilter filter, [FromQuery] PagingModel paging)
        {
            try
            {
                var response = await _recruiterService.ViewAllRecruiters(filter, paging);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch recruiter info list");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpDelete(ApiEndPointConstant.RecruiterInfo.RecruiterInfoEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRecruiterInfo(int id)
        {
            try
            {
                var result = await _recruiterService.DeleteRecruiterInfo(id);
                if (!result)
                {
                    return Ok(new { status = "error", message = "Delete failed" });
                }

                return Ok(new { status = "success", message = "Delete successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete recruiter info");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
