using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPostReportService
    {
        Task<int> CreateNewReport(PostReportInfo request);
        Task<bool> UpdateReportInfo(int id, PostReportInfo request);
        Task<IPaginate<PostReportReponse>> ViewAllReport(PostReportFilter filter, PagingModel pagingModel);
        Task<PostReportReponse> GetReportById(int id);
    }
}
