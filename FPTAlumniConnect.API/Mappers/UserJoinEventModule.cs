using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class UserJoinEventModule :Profile
    {
        public UserJoinEventModule()
        {
            CreateMap<UserJoinEvent, GetUserJoinEventResponse>();
            CreateMap<UserJoinEvent, UserJoinEventInfo>();

            CreateMap<UserJoinEventInfo, UserJoinEvent>();
        }
    }
}
