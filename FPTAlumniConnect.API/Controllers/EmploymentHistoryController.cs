using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class EmploymentHistoryController : BaseController<EmploymentHistoryController>
    {
        private readonly IEmploymentHistoryService _employmentHistoryService;

        public EmploymentHistoryController(ILogger<EmploymentHistoryController> logger,
            IEmploymentHistoryService employmentHistoryService) : base(logger)
        {
            _employmentHistoryService = employmentHistoryService;
        }

        [HttpPost(ApiEndPointConstant.EmploymentHistory.EmploymentHistoriesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmploymentHistory([FromBody] EmploymentHistoryInfo request)
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
                var employmentHistoryId = await _employmentHistoryService.CreateEmploymentHistory(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Employment history created successfully",
                    data = new { id = employmentHistoryId }
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employment history");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        [HttpGet(ApiEndPointConstant.EmploymentHistory.EmploymentHistoryEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmploymentHistoryById(int id)
        {
            try
            {
                var response = await _employmentHistoryService.GetEmploymentHistoryById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch employment history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.EmploymentHistory.EmploymentHistoryEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmploymentHistory(int id, [FromBody] EmploymentHistoryInfo request)
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
                var isSuccessful = await _employmentHistoryService.UpdateEmploymentHistory(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }
                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update employment history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.EmploymentHistory.EmploymentHistoriesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllEmploymentHistories([FromQuery] EmploymentHistoryFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _employmentHistoryService.ViewAllEmploymentHistories(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch employment histories");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpDelete(ApiEndPointConstant.EmploymentHistory.EmploymentHistoryEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmploymentHistory(int id)
        {
            try
            {
                var isSuccessful = await _employmentHistoryService.DeleteEmploymentHistory(id);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Delete failed" });
                }
                return Ok(new { status = "success", message = "Delete successful" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete employment history");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.EmploymentHistory.EmploymentHistoriesByCvEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmploymentHistoriesByCvId(int cvId, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _employmentHistoryService.GetEmploymentHistoriesByCvId(cvId, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch employment histories by CV ID");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}