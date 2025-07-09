using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class GroupChatModules : Profile
    {
        public GroupChatModules() 
        { 
            CreateMap<GroupChat, GroupChatReponse>();
            CreateMap<GroupChatInfo, GroupChat>();

            CreateMap<User, UserResponse>();
        }
    }
}
