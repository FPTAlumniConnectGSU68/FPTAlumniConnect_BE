using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Mappers
{
    public class JobPostModule : Profile
    {
        public JobPostModule()
        {
            // Mapping JobPost → JobPostResponse
            CreateMap<JobPost, JobPostResponse>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major != null ? src.Major.MajorName : null))
                .ForMember(dest => dest.Skills,
                    opt => opt.MapFrom(src => src.JobPostSkills != null
                        ? src.JobPostSkills.Select(jps => new SkillResponse
                        {
                            SkillId = jps.Skill.SkillId,
                            SkillName = jps.Skill.Name
                        }).ToList()
                        : new List<SkillResponse>()));

            // Mapping JobPostInfo → JobPost (for Create/Update)
            CreateMap<JobPostInfo, JobPost>()
                .ForMember(dest => dest.JobPostSkills, opt => opt.Ignore()); // handled manually in service
        }
    }
}
