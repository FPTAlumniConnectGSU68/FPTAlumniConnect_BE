using AutoMapper;
using FirebaseAdmin.Auth.Hash;
using FPTAlumniConnect.BusinessTier.Payload;
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
                 .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName));
            CreateMap<User, GetMentorResponse>()
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                 .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName)); // Assuming 'Name' is the property in the 'Role' entity
            CreateMap<RegisterRequest, User>()
                 .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // New mappings for CreateRecruiterRequest
            CreateMap<CreateRecruiterRequest, User>()
                .ForMember(dest => dest.RoleId, opt => opt.Ignore()) // RoleId will be set manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); // Set in service

            CreateMap<CreateRecruiterRequest, RecruiterInfo>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // Set in service

            CreateMap<User, CreateRecruiterResponse>()
                .ForMember(dest => dest.RecruiterInfoId, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CompanyName, opt => opt.Ignore()); // Set in service
        }
    }
}