using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.NotificationSetting;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class NotificationSettingController : BaseController<NotificationSettingController>
    {
        private readonly INotificationSettingService _notificationSettingService;

        public NotificationSettingController(ILogger<NotificationSettingController> logger, INotificationSettingService notificationSettingService) : base(logger)
        {
            _notificationSettingService = notificationSettingService;
        }

        [HttpGet(ApiEndPointConstant.NotificationSetting.NotificationSettingEndPoint)]
        [ProducesResponseType(typeof(GetNotificationSettingResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotificationSetting(int id)
        {
            var response = await _notificationSettingService.GetNotificationSettingById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.NotificationSetting.NotificationSettingsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetNotificationSettingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllNotificationSettings([FromQuery] NotificationSettingFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _notificationSettingService.ViewAllNotificationSettings(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.NotificationSetting.NotificationSettingsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNotificationSetting([FromBody] NotificationSettingInfo request)
        {
            var id = await _notificationSettingService.CreateNotificationSetting(request);
            return CreatedAtAction(nameof(GetNotificationSetting), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.NotificationSetting.NotificationSettingEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateNotificationSetting(int id, [FromBody] NotificationSettingInfo request)
        {
            var isSuccessful = await _notificationSettingService.UpdateNotificationSetting(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}