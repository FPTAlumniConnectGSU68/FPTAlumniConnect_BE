using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MentorshipSettingsController : ControllerBase
    {
        private readonly IMentorshipSettingsService _settingsService;

        public MentorshipSettingsController(IMentorshipSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("max-per-day")]
        public IActionResult GetMaxPerDay()
        {
            return Ok(new
            {
                status = "success",
                value = _settingsService.GetMaxPerDay()
            });
        }

        [HttpPut("max-per-day")]
        public IActionResult UpdateMaxPerDay([FromQuery] int newValue)
        {
            try
            {
                _settingsService.UpdateMaxPerDay(newValue);
                return Ok(new
                {
                    status = "success",
                    message = $"Max mentorships per day updated to {newValue}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }

}
