using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class CommentModule : Profile
    {
        public CommentModule()
        {
            CreateMap<Comment, CommentReponse>()
                .ForMember(dest => dest.ChildComments, opt => opt.Ignore())

               .ForMember(dest => dest.AuthorName,
                    opt => opt.MapFrom(src => src.Author != null
                        ? $"{src.Author.FirstName} {src.Author.LastName}"
                        : null))
                .ForMember(dest => dest.AuthorAvatar,
                    opt => opt.MapFrom(src => src.Author.ProfilePicture));
            CreateMap<CommentInfo, Comment>();
        }
    }
}
