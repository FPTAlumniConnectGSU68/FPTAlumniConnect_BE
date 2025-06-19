using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class JobApplicationModule : Profile
    {
        public JobApplicationModule()
        {
            CreateMap<JobApplication, JobApplicationResponse>();
            CreateMap<JobApplicationInfo, JobApplication>();
        }
    }
}
