using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class CommentModule : Profile
    {
        public CommentModule()
        {
            CreateMap<Comment, CommentReponse>();
            CreateMap<CommentInfo, Comment>();
        }
    }
}
