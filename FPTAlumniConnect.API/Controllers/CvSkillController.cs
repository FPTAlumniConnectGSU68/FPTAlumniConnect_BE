using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CvSkill;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class CvSkillController : BaseController<CvSkillController>
    {
        private readonly ICvSkillService _cvSkillService;

        public CvSkillController(ILogger<CvSkillController> logger, ICvSkillService cvSkillService) : base(logger)
        {
            _cvSkillService = cvSkillService;
        }

        [HttpPost(ApiEndPointConstant.CvSkill.CvSkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCvSkill([FromBody] CvSkillInfo request)
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
                var id = await _cvSkillService.CreateCvSkill(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { CvId = id, SkillId = request.SkillId }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create CV-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.CvSkill.CvSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCvSkillById([FromQuery] int cvId, [FromQuery] int skillId)
        {
            try
            {
                var response = await _cvSkillService.GetCvSkillById(cvId, skillId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.CvSkill.CvSkillsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllCvSkills([FromQuery] CvSkillFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _cvSkillService.ViewAllCvSkills(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV-Skills");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.CvSkill.CvSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCvSkill([FromQuery] int cvId, [FromQuery] int skillId, [FromBody] CvSkillInfo request)
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
                var isSuccessful = await _cvSkillService.UpdateCvSkill(cvId, skillId, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update CV-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpDelete(ApiEndPointConstant.CvSkill.CvSkillEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCvSkill([FromQuery] int cvId, [FromQuery] int skillId)
        {
            try
            {
                var isSuccessful = await _cvSkillService.DeleteCvSkill(cvId, skillId);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Deletion failed" });
                }

                return Ok(new { status = "success", message = "Deletion successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete CV-Skill");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}