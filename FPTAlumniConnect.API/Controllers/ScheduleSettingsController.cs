using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleSettingsController : ControllerBase
    {
        private readonly IScheduleSettingsService _settingsService;

        public ScheduleSettingsController(IScheduleSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("max-per-day")]
        public IActionResult GetMaxPerDay()
        {
            return Ok(new { value = _settingsService.GetMaxPerDay() });
        }

        [HttpPut("max-per-day")]
        public IActionResult UpdateMaxPerDay([FromQuery] int newValue)
        {
            try
            {
                _settingsService.UpdateMaxPerDay(newValue);
                return Ok(new { message = $"Max schedules per day updated to {newValue}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
