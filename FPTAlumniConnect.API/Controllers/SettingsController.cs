using FPTAlumniConnect.BusinessTier.Configurations;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly MentorshipCleanupSettings _mentorshipCleanup;
        private readonly JobPostCleanupSettings _jobPostCleanup;
        private readonly MentorshipSettings _mentorshipSettings;
        private readonly ScheduleSettings _scheduleSettings;

        public SettingsController(
            IOptions<MentorshipCleanupSettings> mentorshipCleanup,
            IOptions<JobPostCleanupSettings> jobPostCleanup,
            IOptions<MentorshipSettings> mentorshipSettings,
            IOptions<ScheduleSettings> scheduleSettings)
        {
            _mentorshipCleanup = mentorshipCleanup.Value;
            _jobPostCleanup = jobPostCleanup.Value;
            _mentorshipSettings = mentorshipSettings.Value;
            _scheduleSettings = scheduleSettings.Value;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetAllSettings()
        {
            var response = new
            {
                MentorshipCleanup = new { _mentorshipCleanup.Interval },
                JobPostCleanup = new { _jobPostCleanup.Interval },
                MentorshipSettings = new { _mentorshipSettings.MaxPerDay },
                ScheduleSettings = new { _scheduleSettings.MaxPerDay }
            };

            return Ok(new
            {
                status = "success",
                message = "Settings retrieved successfully",
                data = response
            });
        }
    }
}
