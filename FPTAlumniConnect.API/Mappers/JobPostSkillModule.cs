using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.JobPostSkill;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class JobPostSkillModule : Profile
    {
        public JobPostSkillModule() {
            CreateMap<JobPostSkillInfo, JobPostSkill>();
            CreateMap<JobPostSkill, JobPostSkillResponse>()
                .ForMember(dest => dest.JobPostTitle, opt => opt.MapFrom(src => src.JobPost.JobTitle))
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name));
        } 
    }
}
