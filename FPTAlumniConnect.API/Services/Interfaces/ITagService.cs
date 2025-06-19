using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.TagJob;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ITagService
    {
        Task<int> CreateNewTag(TagJobInfo request);
        Task<bool> UpdateTagInfo(int id, TagJobInfo request);
        Task<IPaginate<TagJobReponse>> ViewAllTag(TagJobFilter filter, PagingModel pagingModel);
        Task<TagJobReponse> GetTagById(int id);
        Task<TagJobReponse> GetTagByCvId(int id);
    }
}