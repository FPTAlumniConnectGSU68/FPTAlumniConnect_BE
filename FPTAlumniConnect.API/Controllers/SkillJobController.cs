using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class SkillController : BaseController<SkillController>
    {
        private readonly ISkillService _skillService;

        public SkillController(ILogger<SkillController> logger, ISkillService skillService) : base(logger)
        {
            _skillService = skillService;
        }

        [HttpPost(ApiEndPointConstant.Skill.SkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSkill([FromBody] SkillInfo request)
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
                var id = await _skillService.CreateSkill(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { SkillId = id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Skill.SkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSkillById([FromRoute] int skillId)
        {
            try
            {
                var response = await _skillService.GetSkillById(skillId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Skill.SkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllSkills([FromQuery] SkillFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _skillService.ViewAllSkills(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Skills");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.Skill.SkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSkill([FromRoute] int skillId, [FromBody] SkillInfo request)
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
                var isSuccessful = await _skillService.UpdateSkill(skillId, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpDelete(ApiEndPointConstant.Skill.SkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSkill([FromRoute] int skillId)
        {
            try
            {
                var isSuccessful = await _skillService.DeleteSkill(skillId);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Deletion failed" });
                }

                return Ok(new { status = "success", message = "Deletion successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}