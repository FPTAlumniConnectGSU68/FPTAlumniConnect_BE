using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class MentorshipModule : Profile
    {
        public MentorshipModule()
        {
            CreateMap<Mentorship, MentorshipReponse>()
                .ForMember(dest => dest.AumniId, opt => opt.MapFrom(src => src.AumniId))
                .ForMember(dest => dest.AlumniName, opt => opt.MapFrom(
                    src => src.Aumni != null
                        ? $"{src.Aumni.FirstName} {src.Aumni.LastName}"
                        : null
                ))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules));

            CreateMap<Schedule, ScheduleResponse>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(
                    src => src.Mentor != null
                        ? $"{src.Mentor.FirstName} {src.Mentor.LastName}"
                        : null
                ));

            CreateMap<MentorshipInfo, Mentorship>();
        }
    }
}