using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.EducationHistory;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class EducationHistoryModule: Profile
    {
        public EducationHistoryModule()
        {
            CreateMap<EducationHistory, GetEducationHistoryResponse>();
            CreateMap<EducationHistory, EducationHistoryInfo>();
            CreateMap<EducationHistoryInfo, EducationHistory>();
        }
    }
}
