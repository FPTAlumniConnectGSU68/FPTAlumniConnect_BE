using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class ScheduleModule : Profile
    {
        public ScheduleModule()
        {
            CreateMap<Schedule, ScheduleReponse>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(
                    src => src.Mentor != null
                        ? $"{src.Mentor.FirstName} {src.Mentor.LastName}"
                        : null
                ))
                .ForMember(dest => dest.AlumniName, opt => opt.MapFrom(
                    src => src.MentorShip.Aumni != null
                        ? $"{src.MentorShip.Aumni.FirstName} {src.MentorShip.Aumni.LastName}"
                        : null
                ))
                .ForMember(dest => dest.AlumniId, opt => opt.MapFrom(
                    src => src.MentorShip != null
                        ? src.MentorShip.AumniId
                        : null
                ))
                .ForMember(dest => dest.RequestMessage, opt => opt.MapFrom(
                    src => src.MentorShip.RequestMessage != null
                        ? src.MentorShip.RequestMessage
                        : null))
                .ForMember(dest => dest.ResultMessage, opt => opt.MapFrom(
                    src => src.MentorShip.ResultMessage != null
                        ? src.MentorShip.ResultMessage
                        : null));

            CreateMap<ScheduleInfo, Schedule>();
        }
    }
}
