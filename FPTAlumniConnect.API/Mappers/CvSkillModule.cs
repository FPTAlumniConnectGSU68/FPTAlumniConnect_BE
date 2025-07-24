using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.CvSkill;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class CvSkillModule : Profile
    {
        public CvSkillModule() {
            CreateMap<CvSkillInfo, CvSkill>();
            CreateMap<CvSkill, CvSkillResponse>()
                .ForMember(dest => dest.CvTitle, opt => opt.MapFrom(src => src.Cv.FullName))
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name));
        }
    }
}
