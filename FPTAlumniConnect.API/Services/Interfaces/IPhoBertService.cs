
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPhoBertService
    {
        Task<List<Cv>> RecommendCVForJobPostAsync(int jobPostId);
    }
}
