using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class SkillModule : Profile
    {
        public SkillModule()
        {
            CreateMap<SkillJob, SkillJobReponse>();
            CreateMap<SkillJobInfo, SkillJob>();
        }
    }
}
