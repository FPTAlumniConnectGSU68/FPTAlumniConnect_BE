using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.WorkExperience;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class WorkExperienceModule : Profile
    {
        public WorkExperienceModule()
        {
            // Mapping from WorkExperience model to WorkExperienceResponse DTO
            CreateMap<WorkExperience, WorkExperienceResponse>()
                      .ForMember(dest => dest.UserName,
                          opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            // Mapping from WorkExperienceInfo DTO to WorkExperience model
            CreateMap<WorkExperienceInfo, WorkExperience>();
        }
    }
}