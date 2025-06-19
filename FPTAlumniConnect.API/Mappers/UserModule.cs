using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class UserModule : Profile
    {
        public UserModule()
        {
            CreateMap<User, GetUserResponse>()
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                 .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName));  // Assuming 'Name' is the property in the 'Role' entity
            CreateMap<RegisterRequest, User>()
          .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
          .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}