using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IMessageGroupChatService
    {
        Task<int> CreateMessageGroupChat(MessageGroupChatInfo request);
        Task<MessageGroupChatInfo> GetMessageGroupChatById(int id);
        Task<bool> UpdateMessageGroupChat(int id, MessageGroupChatInfo request);
        Task<IPaginate<MessageGroupChatInfo>> ViewAllMessagesInGroupChat(MessageGroupChatFilter filter, PagingModel pagingModel);
    }
}
