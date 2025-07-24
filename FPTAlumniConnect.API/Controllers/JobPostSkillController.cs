using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPostSkill;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class JobPostSkillController : BaseController<JobPostSkillController>
    {
        private readonly IJobPostSkillService _jobPostSkillService;

        public JobPostSkillController(ILogger<JobPostSkillController> logger, IJobPostSkillService jobPostSkillService)
            : base(logger)
        {
            _jobPostSkillService = jobPostSkillService;
        }

        [HttpPost(ApiEndPointConstant.JobPostSkill.JobPostSkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateJobPostSkill([FromBody] JobPostSkillInfo request)
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
                var id = await _jobPostSkillService.CreateJobPostSkill(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { JobPostId = id, SkillId = request.SkillId }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create JobPost-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.JobPostSkill.JobPostSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobPostSkillById([FromRoute] int jobPostId, [FromRoute] int skillId)
        {
            try
            {
                var response = await _jobPostSkillService.GetJobPostSkillById(jobPostId, skillId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch JobPost-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.JobPostSkill.JobPostSkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllJobPostSkills([FromQuery] JobPostSkillFilter filter,
            [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _jobPostSkillService.ViewAllJobPostSkills(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch JobPost-Skills");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.JobPostSkill.JobPostSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateJobPostSkill([FromRoute] int jobPostId, [FromRoute] int skillId,
            [FromBody] JobPostSkillInfo request)
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
                var isSuccessful = await _jobPostSkillService.UpdateJobPostSkill(jobPostId, skillId, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update JobPost-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpDelete(ApiEndPointConstant.JobPostSkill.JobPostSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteJobPostSkill([FromRoute] int jobPostId, [FromRoute] int skillId)
        {
            try
            {
                var isSuccessful = await _jobPostSkillService.DeleteJobPostSkill(jobPostId, skillId);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Deletion failed" });
                }

                return Ok(new { status = "success", message = "Deletion successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete JobPost-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}