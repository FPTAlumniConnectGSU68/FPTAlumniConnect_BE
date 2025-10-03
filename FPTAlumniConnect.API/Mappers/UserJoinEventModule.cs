using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class UserJoinEventModule : Profile
    {
        public UserJoinEventModule()
        {
            CreateMap<UserJoinEvent, GetUserJoinEventResponse>()
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src =>
                        src.Event != null && src.Event.UserJoinEvents.Any(u => u.Rating.HasValue)
                            ? Math.Round(
                                src.Event.UserJoinEvents
                                   .Where(u => u.Rating.HasValue)
                                   .Average(u => u.Rating.Value),
                                1)
                            : (double?)null));

            CreateMap<UserJoinEvent, UserJoinEventInfo>();

            CreateMap<UserJoinEventInfo, UserJoinEvent>();
        }
    }
}
