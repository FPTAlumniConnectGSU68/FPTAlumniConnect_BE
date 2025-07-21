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
                .ForMember(dest => dest.AlumniName, opt => opt.MapFrom(
                    src => src.Aumni != null
                        ? $"{src.Aumni.FirstName} {src.Aumni.LastName}"
                        : null
                ));

            CreateMap<MentorshipInfo, Mentorship>();
        }
    }
}
