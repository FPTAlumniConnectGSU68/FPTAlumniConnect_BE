using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IUserJoinEventService
    {
        Task<int> CreateNewUserJoinEvent(UserJoinEventInfo request);
        Task<GetUserJoinEventResponse> GetUserJoinEventById(int id);
        Task<bool> UpdateUserJoinEvent(int id, UserJoinEventInfo request);
        Task<IPaginate<GetUserJoinEventResponse>> ViewAllUserJoinEvents(UserJoinEventFilter filter, PagingModel pagingModel);
    }
}
