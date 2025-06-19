using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ITimeLineService
    {
        Task<int> CreateTimeLine(TimeLineInfo request);
        Task<TimeLineReponse> GetTimeLineById(int id);
        Task<bool> UpdateTimeLine(int id, TimeLineInfo request);
        Task<IPaginate<TimeLineReponse>> ViewAllTimeLine(TimeLineFilter filter, PagingModel pagingModel);
    }
}