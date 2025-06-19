using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IGroupChatService
    {
        Task<int> CreateGroupChat(GroupChatInfo request);
        Task<GroupChatReponse> GetGroupChatById(int id);
        Task<bool> UpdateGroupChat(int id, GroupChatInfo request);
        Task<IPaginate<GroupChatReponse>> ViewAllMessagesInGroupChat(GroupChatFilter filter, PagingModel pagingModel);
    }
}
