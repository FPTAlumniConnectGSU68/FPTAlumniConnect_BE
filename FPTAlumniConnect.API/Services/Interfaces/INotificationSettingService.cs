using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.NotificationSetting;
using FPTAlumniConnect.DataTier.Paginate;

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