using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ISocialLinkService
    {
        Task<int> CreateSocialLink(SocialLinkInfo request);
        Task<IPaginate<GetSocialLinkResponse>> ViewAllSocialLinks(SocialLinkFilter filter, PagingModel pagingModel);
        Task<bool> UpdateSocialLink(int id, SocialLinkInfo request);
        Task<GetSocialLinkResponse> GetSocialLinkById(int id);
        Task<bool> DeleteSocialLink(int id);
    }
}