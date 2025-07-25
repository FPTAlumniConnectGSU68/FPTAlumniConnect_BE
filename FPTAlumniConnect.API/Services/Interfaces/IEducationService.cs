using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Education;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IEducationService
    {
        Task<int> CreateEducationAsync(EducationInfo request);
        Task<EducationResponse> GetEducationByIdAsync(int id);
        Task<bool> UpdateEducationAsync(int id, EducationInfo request);
        Task<bool> DeleteEducationAsync(int id);
        Task<IPaginate<EducationResponse>> ViewAllEducationAsync(EducationFilter filter, PagingModel pagingModel);

        Task<List<EducationStatisticsDto>> GetEducationStatsByUser(int userId, string groupBy);
    }
}