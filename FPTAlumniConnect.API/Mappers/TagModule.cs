using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.TagJob;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class TagModule : Profile
    {
        public TagModule()
        {
            CreateMap<TagJob, TagJobReponse>();
            CreateMap<TagJobInfo, TagJob>();
        }
    }
}
