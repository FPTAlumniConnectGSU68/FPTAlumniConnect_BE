using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.NotificationSetting;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class NotificationSettingModule: Profile
    {
        public NotificationSettingModule() 
        {
            CreateMap<NotificationSetting, GetNotificationSettingResponse>();
            CreateMap<NotificationSettingInfo, NotificationSetting>();
        }
    }
}
