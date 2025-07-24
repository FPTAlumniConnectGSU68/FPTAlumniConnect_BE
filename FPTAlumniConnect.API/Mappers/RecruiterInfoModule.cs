using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class RecruiterInfoModule : Profile
    {
        public RecruiterInfoModule()
        {
            CreateMap<RecruiterInfo, RecruiterInfoResponse>();
            CreateMap<RecruiterInfoInfo, RecruiterInfo>();
        }
    }
}
