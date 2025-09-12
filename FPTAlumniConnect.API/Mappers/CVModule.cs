using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class CVModule : Profile
    {
        public CVModule()
        {
            // Mapping from CVInfo (DTO) to Cv (Model)
            CreateMap<CVInfo, Cv>()
                .ForMember(dest => dest.CvSkills, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentHistories,
                    opt => opt.MapFrom(src => src.EmploymentHistories))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status != null
                        ? Enum.Parse<CVStatus>(src.Status)
                        : CVStatus.Pending));

            // Mapping from EmploymentHistoryInfo (DTO) to EmploymentHistory (Model)
            CreateMap<EmploymentHistoryInfo, EmploymentHistory>();

            // Mapping from Cv (Model) to CVResponse (DTO)
            CreateMap<Cv, CVResponse>()
                .ForMember(dest => dest.SkillIds,
                    opt => opt.MapFrom(src => src.CvSkills.Select(cs => cs.SkillId).ToList()))
                .ForMember(dest => dest.SkillNames,
                    opt => opt.MapFrom(src => src.CvSkills.Select(cs => cs.Skill.Name).ToList()))
                .ForMember(dest => dest.MajorId,
                    opt => opt.MapFrom(src => src.MajorId))
                .ForMember(dest => dest.MajorName,
                    opt => opt.MapFrom(src => src.Major != null ? src.Major.MajorName : null))
                .ForMember(dest => dest.EmploymentHistories,
                    opt => opt.MapFrom(src => src.EmploymentHistories));

            // Mapping from EmploymentHistory (Model) to EmploymentHistoryResponse (DTO)
            CreateMap<EmploymentHistory, EmploymentHistoryResponse>();
        }
    }
}