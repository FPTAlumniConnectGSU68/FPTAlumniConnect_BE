using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EducationHistory;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IEducationHistoryService
    {
        Task<int> CreateNewEducationHistory(EducationHistoryInfo request);
        Task<IPaginate<GetEducationHistoryResponse>> ViewAllEducationHistory(EducationHistoryFilter filter, PagingModel pagingModel);
        Task<bool> UpdateEducationHistory(int id, EducationHistoryInfo request);
        Task<GetEducationHistoryResponse> GetEducationHistoryById(int id);
    }
}