using FPTAlumniConnect.API.Services;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Configurations;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : BaseController<SettingsController>
    {
        private readonly MentorshipCleanupSettings _mentorshipCleanup;
        private readonly JobPostCleanupSettings _jobPostCleanup;
        private readonly MentorshipSettings _mentorshipSettings;
        private readonly ScheduleSettings _scheduleSettings;
        private readonly MentorshipCleanupService _mentorshipCleanupService;
        private readonly JobPostCleanupService _jobPostCleanupService;

        public SettingsController(
            ILogger<SettingsController> logger,
            IOptions<MentorshipCleanupSettings> mentorshipCleanup,
            IOptions<JobPostCleanupSettings> jobPostCleanup,
            IOptions<MentorshipSettings> mentorshipSettings,
            IOptions<ScheduleSettings> scheduleSettings,
            JobPostCleanupService jobPostCleanupService,
            MentorshipCleanupService mentorshipCleanupService) : base(logger)
        {
            _mentorshipCleanup = mentorshipCleanup?.Value ?? throw new ArgumentNullException(nameof(mentorshipCleanup));
            _jobPostCleanup = jobPostCleanup?.Value ?? throw new ArgumentNullException(nameof(jobPostCleanup));
            _mentorshipSettings = mentorshipSettings?.Value ?? throw new ArgumentNullException(nameof(mentorshipSettings));
            _scheduleSettings = scheduleSettings?.Value ?? throw new ArgumentNullException(nameof(scheduleSettings));
            _jobPostCleanupService = jobPostCleanupService ?? throw new ArgumentNullException(nameof(jobPostCleanupService));
            _mentorshipCleanupService = mentorshipCleanupService ?? throw new ArgumentNullException(nameof(mentorshipCleanupService));
        }

        [HttpGet(ApiEndPointConstant.Settings.SettingsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllSettings()
        {
            try
            {
                var jobPostInterval = _jobPostCleanupService.GetInterval();
                var mentorshipInterval = _mentorshipCleanupService.GetInterval();
                var response = new
                {
                    MentorshipCleanup = new { Interval = mentorshipInterval.TotalHours },
                    JobPostCleanup = new { Interval = jobPostInterval.TotalHours },
                    MentorshipSettings = new { MaxPerDay = _mentorshipSettings.MaxPerDay },
                    ScheduleSettings = new { MaxPerDay = _scheduleSettings.MaxPerDay }
                };

                return Ok(new
                {
                    status = "success",
                    message = "Settings retrieved successfully",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch settings");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }
    }
}