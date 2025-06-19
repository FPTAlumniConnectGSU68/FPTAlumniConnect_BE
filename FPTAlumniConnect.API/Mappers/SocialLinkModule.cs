using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class SocialLinkModule : Profile
    {
        public SocialLinkModule() 
        {
            CreateMap<SoicalLink, GetSocialLinkResponse>();
            CreateMap<SocialLinkInfo, SoicalLink>();
        }
    }
}
