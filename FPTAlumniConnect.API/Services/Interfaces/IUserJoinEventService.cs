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
    }
}
