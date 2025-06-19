using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPostService
    {
        Task<int> CreateNewPost(PostInfo request);
        Task<IPaginate<PostReponse>> ViewAllPost(PostFilter filter, PagingModel pagingModel);
        Task<bool> UpdatePostInfo(int id, PostInfo request);
        Task<PostReponse> GetPostById(int id);
    }
}
