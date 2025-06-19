using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.PrivacySetting;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PrivacySettingController : BaseController<PrivacySettingController>
    {
        private readonly IPrivacySettingService _privacySettingService;

        public PrivacySettingController(ILogger<PrivacySettingController> logger, IPrivacySettingService privacySettingService) : base(logger)
        {
            _privacySettingService = privacySettingService;
        }

        [HttpGet(ApiEndPointConstant.PrivacySetting.PrivacySettingEndPoint)]
        [ProducesResponseType(typeof(GetPrivacySettingResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPrivacySetting(int id)
        {
            var response = await _privacySettingService.GetPrivacySettingById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.PrivacySetting.PrivacySettingsEndPoint)]
        [ProducesResponseType(typeof(GetPrivacySettingResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllPrivacySettings([FromQuery] PrivacySettingFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _privacySettingService.ViewAllPrivacySettings(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.PrivacySetting.PrivacySettingsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePrivacySetting([FromBody] PrivacySettingInfo request)
        {
            var id = await _privacySettingService.CreatePrivacySetting(request);
            return CreatedAtAction(nameof(GetPrivacySetting), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.PrivacySetting.PrivacySettingEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePrivacySetting(int id, [FromBody] PrivacySettingInfo request)
        {
            var isSuccessful = await _privacySettingService.UpdatePrivacySetting(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}