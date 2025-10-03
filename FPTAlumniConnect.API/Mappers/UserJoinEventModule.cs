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
                            : (double?)null))

                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.User.Code));
            CreateMap<UserJoinEvent, UserJoinEventInfo>();
            CreateMap<UserJoinEventInfo, UserJoinEvent>();
        }
    }
}
