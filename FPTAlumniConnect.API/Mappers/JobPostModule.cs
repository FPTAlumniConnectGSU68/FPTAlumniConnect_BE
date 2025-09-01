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
                            Name = jps.Skill.Name
                        }).ToList()
                        : new List<SkillResponse>()))
                .ForMember(dest => dest.RecruiterInfoId, opt => opt.MapFrom(src => src.User != null && src.User.RecruiterInfos != null ? src.User.RecruiterInfos.RecruiterInfoId : (int?)null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.User != null && src.User.RecruiterInfos != null ? src.User.RecruiterInfos.CompanyName : null))
                .ForMember(dest => dest.CompanyLogoUrl, opt => opt.MapFrom(src => src.User != null && src.User.RecruiterInfos != null ? src.User.RecruiterInfos.CompanyLogoUrl : null));

            // Mapping JobPostInfo → JobPost (for Create/Update)
            CreateMap<JobPostInfo, JobPost>()
                .ForMember(dest => dest.JobPostSkills, opt => opt.Ignore()); // handled manually in service
        }
    }
}
