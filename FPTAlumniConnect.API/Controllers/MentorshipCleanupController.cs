using FPTAlumniConnect.API.Services;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class MentorshipCleanupController : BaseController<MentorshipCleanupController>
    {
        private readonly MentorshipCleanupService _cleanupService;

        public MentorshipCleanupController(ILogger<MentorshipCleanupController> logger, MentorshipCleanupService cleanupService)
            : base(logger)
        {
            _cleanupService = cleanupService ?? throw new ArgumentNullException(nameof(cleanupService));
        }

        [HttpGet(ApiEndPointConstant.MentorshipCleanup.IntervalEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult GetCleanupInterval()
        {
            try
            {
                var interval = _cleanupService.GetInterval();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = new { hours = interval.TotalHours }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch cleanup interval");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

        [HttpPut(ApiEndPointConstant.MentorshipCleanup.IntervalEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCleanupInterval([FromBody] UpdateIntervalRequest request)
        {
            if (request == null || request.Hours <= 0)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Interval hours must be greater than zero and request body must not be null" }
                });
            }

            try
            {
                var newInterval = TimeSpan.FromHours(request.Hours);
                _cleanupService.UpdateInterval(newInterval);
                return Ok(new
                {
                    status = "success",
                    message = "Cleanup interval updated successfully",
                    data = new { hours = request.Hours }
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Failed to update cleanup interval");
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cleanup interval");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }
    }
}