using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<int> CreateNewEvent(EventInfo request);
        Task<GetEventResponse> GetEventById(int id);
        Task<bool> UpdateEventInfo(int id, EventInfo request);
        Task<IPaginate<GetEventResponse>> ViewAllEvent(EventFilter filter, PagingModel pagingModel);
    }
}