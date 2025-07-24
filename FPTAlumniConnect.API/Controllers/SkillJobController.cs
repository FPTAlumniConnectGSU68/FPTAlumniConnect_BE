using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class SkillJobController : BaseController<SkillJobController>
    {
        private readonly ISkillService _skillService;

        public SkillJobController(ILogger<SkillJobController> logger, ISkillService skillService)
            : base(logger)
        {
            _skillService = skillService;
        }

        // Add or assign a skill to a CV
        [HttpPost(ApiEndPointConstant.Skill.SkillsEndPoint)]
        public async Task<IActionResult> CreateNewSkill([FromBody] SkillJobInfo request)
        {
            if (request == null)
            {
                return BadRequest(new { status = "error", message = "Request body is null" });
            }

            try
            {
                var id = await _skillService.CreateNewSkill(request);
                return StatusCode(201, new { status = "success", message = "Skill created", data = new { id } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Get all skills assigned to a CV
        [HttpGet(ApiEndPointConstant.Skill.SkillCVEndPoint)]
        public async Task<IActionResult> GetSkillsByCvId([FromQuery] int id)
        {
            try
            {
                var response = await _skillService.GetSkillsByCvId(id);
                return Ok(new { status = "success", data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch skills by CV ID");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Delete a skill from a CV
        [HttpDelete(ApiEndPointConstant.Skill.SkillEndPoint)]
        public async Task<IActionResult> DeleteSkillFromCv([FromQuery] int skillId, [FromQuery] int cvId)
        {
            try
            {
                var success = await _skillService.DeleteSkillFromCv(skillId, cvId);
                if (!success) return BadRequest(new { status = "error", message = "Delete failed" });

                return Ok(new { status = "success", message = "Skill removed from CV" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete skill from CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Count skills of a CV
        [HttpGet(ApiEndPointConstant.Skill.SkillCountEndPoint)]
        public async Task<IActionResult> CountSkillByCvId([FromQuery] int cvId)
        {
            try
            {
                int count = await _skillService.CountSkillByCvId(cvId);
                return Ok(new { status = "success", data = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to count skills");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
