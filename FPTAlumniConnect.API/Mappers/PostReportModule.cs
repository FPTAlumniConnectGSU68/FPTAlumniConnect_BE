using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class PostReportModule : Profile
    {
        public PostReportModule()
        {
            CreateMap<PostReport, PostReportReponse>();
            CreateMap<PostReportInfo, PostReport>();
        }
    }
}
