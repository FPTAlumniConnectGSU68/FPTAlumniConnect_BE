using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class CVModule : Profile
    {
        public CVModule()
        {
            CreateMap<Cv, CVReponse>();
            CreateMap<CVInfo, Cv>();
        }
    }
}
