using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class PostModule : Profile
    {
        public PostModule()
        {
            CreateMap<Post, PostReponse>()
                         .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName)); ;
            CreateMap<PostInfo, Post>();
        }
    }
}