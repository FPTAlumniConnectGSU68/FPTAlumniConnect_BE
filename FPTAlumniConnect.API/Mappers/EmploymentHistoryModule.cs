using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class EmploymentHistoryModule : Profile
    {
        public EmploymentHistoryModule()
        {
            CreateMap<EmploymentHistoryInfo, EmploymentHistory>();
            CreateMap<EmploymentHistory, EmploymentHistoryResponse>();
        }
    }
}