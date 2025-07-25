using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class NotificationModule : Profile
    {
        public NotificationModule()
        {
            CreateMap<Notification, NotificationResponse>();
            CreateMap<NotificationPayload, Notification>();
        }
    }
}
