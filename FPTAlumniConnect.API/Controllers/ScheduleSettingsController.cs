using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleSettingsController : BaseController<ScheduleSettingsController>
    {
        private readonly IScheduleSettingsService _settingsService;

        public ScheduleSettingsController(
            ILogger<ScheduleSettingsController> logger,
            IScheduleSettingsService settingsService) : base(logger)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        [HttpGet(ApiEndPointConstant.ScheduleSettings.MaxPerDayEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult GetMaxPerDay()
        {
            try
            {
                var maxPerDay = _settingsService.GetMaxPerDay();
                return Ok(new
                {
                    status = "success",
                    message = "Max schedules per day retrieved successfully",
                    data = new { MaxPerDay = maxPerDay }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch max schedules per day");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

        [HttpPut(ApiEndPointConstant.ScheduleSettings.MaxPerDayEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMaxPerDay([FromQuery] int newValue)
        {
            if (newValue <= 0)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Max schedules per day must be greater than 0" }
                });
            }

            try
            {
                _settingsService.UpdateMaxPerDay(newValue);
                return Ok(new
                {
                    status = "success",
                    message = $"Max schedules per day updated to {newValue}",
                    data = new { MaxPerDay = newValue }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update max schedules per day");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error",
                    errors = new[] { ex.Message } // Show service errors as per style guide
                });
            }
        }
    }
}