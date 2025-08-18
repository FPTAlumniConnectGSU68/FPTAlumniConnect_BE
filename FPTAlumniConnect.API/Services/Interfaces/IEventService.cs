using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<int> CreateNewEvent(EventInfo request); 
        Task<GetEventResponse> GetEventById(int id);
        Task<EventDetailResponse> GetEventByIdAsync(int eventId);
        Task<bool> UpdateEventInfo(int id, EventInfo request);
        Task<IPaginate<GetEventResponse>> ViewAllEvent(EventFilter filter, PagingModel pagingModel);
        Task<int> CountAllEvents();
        Task<int> CountEventsByMonth(int month, int year);
        Task<Dictionary<string, int>> GetEventCountByStatus();

        Task<bool> CheckEventConflict(int eventId, DateTime newStart, DateTime newEnd);

        Task<IEnumerable<GetEventResponse>> GetSimilarEvents(int eventId, int count);

        Task<IEnumerable<EventPopularityDto>> GetEventsByPopularity(int top);

        Task<BestEventTimeDto> SuggestBestTimeForNewEvent(int organizerId, int durationHours);

        List<SuggestedTimelineDto> GetSuggestedTimelines(DateTime eventStartTime, int eventDurationHours);

        Task<IPaginate<GetEventResponse>> GetEventsUserJoined(int userId, EventFilter filter, PagingModel pagingModel);
    }
}