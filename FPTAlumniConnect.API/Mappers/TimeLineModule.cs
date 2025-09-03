using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class TimeLineModule : Profile
    {
        public TimeLineModule()
        {
            // Map from EventTimeLine to TimeLineReponse
            CreateMap<EventTimeLine, TimeLineReponse>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm\:ss")));

            // Map from TimeLineInfo to EventTimeLine
            CreateMap<TimeLineInfo, EventTimeLine>();
        }
    }
}
