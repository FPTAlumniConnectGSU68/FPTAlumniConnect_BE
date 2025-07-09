using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IGroupChatService
    {
        Task<int> CreateGroupChat(GroupChatInfo request);
        Task<GroupChatReponse> GetGroupChatById(int id);
        Task<bool> UpdateGroupChat(int id, GroupChatInfo request);
        Task<IPaginate<GroupChatReponse>> ViewAllMessagesInGroupChat(GroupChatFilter filter, PagingModel pagingModel);

        Task<bool> DeleteGroupChat(int id);
        Task<bool> AddUserToGroup(int groupId, int userId);
        Task<bool> LeaveGroup(int groupId, int userId);
        Task<List<GroupChatReponse>> GetGroupsByUserId(int userId);
        Task<List<UserResponse>> GetMembersInGroup(int groupId);
        Task<List<GroupChatReponse>> SearchGroupsByName(string keyword);

    }
}
