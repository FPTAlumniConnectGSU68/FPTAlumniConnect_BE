using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IEmploymentHistoryService
    {
        Task<int> CreateEmploymentHistory(EmploymentHistoryInfo request);
        Task<EmploymentHistoryResponse> GetEmploymentHistoryById(int employmentHistoryId);
        Task<bool> UpdateEmploymentHistory(int employmentHistoryId, EmploymentHistoryInfo request);
        Task<IPaginate<EmploymentHistoryResponse>> ViewAllEmploymentHistories(EmploymentHistoryFilter filter, PagingModel pagingModel);
        Task<bool> DeleteEmploymentHistory(int employmentHistoryId);
        Task<IPaginate<EmploymentHistoryResponse>> GetEmploymentHistoriesByCvId(int cvId, PagingModel pagingModel);
    }
}
