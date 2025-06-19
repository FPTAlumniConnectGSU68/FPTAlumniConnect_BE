using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.SpMajorCode;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class SpMajorCodeModule : Profile
    {
        public SpMajorCodeModule()
        {
            CreateMap<SpMajorCode, SpMajorCodeResponse>();
            CreateMap<SpMajorCodeInfo, SpMajorCode>();
        }
    }
}
