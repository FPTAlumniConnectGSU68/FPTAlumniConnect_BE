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
                         .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName))
                         .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author != null
                    ? $"{src.Author.FirstName} {src.Author.LastName}"
                    : null))
            .ForMember(dest => dest.AuthorAvatar,
                opt => opt.MapFrom(src => src.Author.ProfilePicture));
            CreateMap<PostInfo, Post>();
        }
    }
}