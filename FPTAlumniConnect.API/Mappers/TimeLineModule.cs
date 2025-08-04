using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class TimeLineModule : Profile
    {
        public TimeLineModule()
        {
            CreateMap<EventTimeLine, TimeLineReponse>();

            // Ánh xạ từ TimeLineInfo sang EventTimeLine
            CreateMap<TimeLineInfo, EventTimeLine>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.TimeOfDay))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.TimeOfDay));
        }
    }
}
