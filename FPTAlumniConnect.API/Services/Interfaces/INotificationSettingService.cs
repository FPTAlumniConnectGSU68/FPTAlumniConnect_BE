using FPTAlumniConnect.BusinessTier.Payload.NotificationSetting;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface INotificationSettingService
    {
        Task<int> CreateNotificationSetting(NotificationSettingInfo request);
        Task<IPaginate<GetNotificationSettingResponse>> ViewAllNotificationSettings(NotificationSettingFilter filter, PagingModel pagingModel);
        Task<bool> UpdateNotificationSetting(int id, NotificationSettingInfo request);
        Task<GetNotificationSettingResponse> GetNotificationSettingById(int id);
    }
}