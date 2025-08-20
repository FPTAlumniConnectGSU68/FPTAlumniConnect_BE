using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IUserJoinEventService
    {
        Task<int> CreateNewUserJoinEvent(UserJoinEventInfo request);
        Task<GetUserJoinEventResponse> GetUserJoinEventById(int id);
        Task<bool> UpdateUserJoinEvent(int id, UserJoinEventInfo request);
        Task<IPaginate<GetUserJoinEventResponse>> ViewAllUserJoinEvents(UserJoinEventFilter filter, PagingModel pagingModel);
        Task<int> GetTotalParticipants(int eventId);
        Task<Dictionary<string, int>> GetTotalParticipantsByRole(int eventId);
        Task<Dictionary<string, int>> GetTotalParticipantsByDay(int eventId);
        Task<IEnumerable<GetUserJoinEventResponse>> GetEvaluations(int eventId);
        Task<bool> CheckUserParticipation(int userId, int eventId);
    }
}
