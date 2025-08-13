using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPhoBertService
    {
        Task<IPaginate<CVResponse>> RecommendCVForJobPostAsync(int jobPostId, PagingModel pagingModel);
    }
}
