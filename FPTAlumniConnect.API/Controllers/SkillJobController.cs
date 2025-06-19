using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class SkillJobController : BaseController<SkillJobController>
    {
        private readonly ISkillService _skillService;

        public SkillJobController(ILogger<SkillJobController> logger, ISkillService skillService) : base(logger)
        {
            _skillService = skillService;
        }

        [HttpGet(ApiEndPointConstant.Skill.SkillEndPoint)]
        [ProducesResponseType(typeof(SkillJobReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var response = await _skillService.GetSkillById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Skill.SkillCVEndPoint)]
        [ProducesResponseType(typeof(SkillJobReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSkillByCvId(int id)
        {
            var response = await _skillService.GetSkillByCvId(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Skill.SkillsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewSkill([FromBody] SkillJobInfo request)
        {
            var id = await _skillService.CreateNewSkill(request);
            return CreatedAtAction(nameof(GetSkillById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Skill.SkillsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<SkillJobReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllSkill([FromQuery] SkillJobFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _skillService.ViewAllSkill(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Skill.SkillEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSkillInfo(int id, [FromBody] SkillJobInfo request)
        {
            var isSuccessful = await _skillService.UpdateSkillInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
