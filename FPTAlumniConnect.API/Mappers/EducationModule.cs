using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Education;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class EducationModule : Profile
    {
        public EducationModule()
        {
            CreateMap<Education, EducationResponse>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<EducationInfo, Education>();
        }
    }
}