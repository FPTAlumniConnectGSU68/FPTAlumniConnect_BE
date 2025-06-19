using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class JobPostModule : Profile
    {
        public JobPostModule()
        {
            CreateMap<JobPost, JobPostResponse>()
                 .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.MajorName)); ;
            CreateMap<JobPostInfo, JobPost>();
        }
    }
}