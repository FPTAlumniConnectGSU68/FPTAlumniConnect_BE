using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.BusinessTier.Payload.SpMajorCode;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class TimeLineModule : Profile
    {
        public TimeLineModule()
        {
            CreateMap<EventTimeLine, TimeLineReponse>();
            CreateMap<TimeLineInfo, EventTimeLine>();
        }
    }
}
