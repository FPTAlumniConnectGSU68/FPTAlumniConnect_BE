using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IRecruiterInfoService
    {
        Task<int> CreateNewRecruiterInfo(RecruiterInfoInfo request);
        Task<bool> UpdateRecruiterInfo(int id, RecruiterInfoInfo request);

        Task<IPaginate<RecruiterInfoResponse>> ViewAllRecruiters(RecruiterInfoFilter filter, PagingModel paging);

        Task<RecruiterInfoResponse> GetRecruiterInfoByUserId(int userId);

        Task<RecruiterInfoResponse> GetRecruiterInfoById(int id);

        Task<bool> DeleteRecruiterInfo(int id);

        Task<bool> UpdateRecruiterStatus(int id, string status);
    }
}