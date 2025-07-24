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
                ));

            CreateMap<ScheduleInfo, Schedule>();
        }
    }
}
