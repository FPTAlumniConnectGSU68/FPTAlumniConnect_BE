using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> CreateNewComment(CommentInfo request);
        Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel);
        Task<bool> UpdateCommentInfo(int id, CommentInfo request);
        Task<CommentReponse> GetCommentById(int id);
    }
}
