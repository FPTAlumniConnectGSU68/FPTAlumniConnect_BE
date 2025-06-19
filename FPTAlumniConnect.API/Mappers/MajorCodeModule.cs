using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class MajorCodeModule : Profile
    {
        public MajorCodeModule()
        {
            CreateMap<MajorCode, MajorCodeReponse>();
            CreateMap<MajorCodeInfo, MajorCode>();
        }
    }
}
