using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class MessageGroupChatModule : Profile
    {
        public MessageGroupChatModule()
        {
            //CreateMap<MessageGroupChat, MessageGroupChatReponse>();
            CreateMap<MessageGroupChatInfo, MessageGroupChat>();
        }
    }
}
