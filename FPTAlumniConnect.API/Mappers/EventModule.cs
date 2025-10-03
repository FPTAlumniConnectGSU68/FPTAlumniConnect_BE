using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class EventModule : Profile
    {
        public EventModule()
        {
            CreateMap<Event, GetEventResponse>()
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src =>
                        src.UserJoinEvents != null && src.UserJoinEvents.Any(u => u.Rating.HasValue)
                            ? Math.Round(
                                src.UserJoinEvents
                                   .Where(u => u.Rating.HasValue)
                                   .Average(u => u.Rating.Value),
                                1)
                            : (double?)null))

                .ForMember(dest => dest.UserJoinEventCount,
                        opt => opt.MapFrom(src => src.UserJoinEvents.Count));

            CreateMap<EventInfo, Event>();
            CreateMap<UserJoinEvent, GetEventResponse>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.Event.EventId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.EventName))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Event.Img))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Event.Description))
                .ForMember(dest => dest.Speaker, opt => opt.MapFrom(src => src.Event.Speaker))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Event.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Event.EndDate))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Event.Location))
                .ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.Event.OrganizerId))
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(src => src.Event.MajorId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Event.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Event.UpdatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Event.Status))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Event.CreatedBy))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Event.UserJoinEvents.Any()
                        ? src.Event.UserJoinEvents.Average(u => u.Rating ?? 0)
                        : (double?)null))
                .ForMember(dest => dest.UserJoinEventCount,
                    opt => opt.MapFrom(src => src.Event.UserJoinEvents.Count));

            CreateMap<EventTimeLineInfo, EventTimeLine>();
            CreateMap<Event, EventDetailResponse>()
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src =>
                        src.UserJoinEvents != null && src.UserJoinEvents.Any(u => u.Rating.HasValue)
                            ? Math.Round(
                                src.UserJoinEvents
                                   .Where(u => u.Rating.HasValue)
                                   .Average(u => u.Rating.Value),
                                1)
                            : (double?)null))

                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(
                    src => src.Organizer != null
                        ? src.Organizer.FirstName + " " + src.Organizer.LastName
                        : null
                ))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major != null ? src.Major.MajorName : null))
                .ForMember(dest => dest.EventTimeLines, opt => opt.MapFrom(src => src.EventTimeLines));
            // EventTimeLine -> EventTimeLineResponse
            CreateMap<EventTimeLine, EventTimeLineResponse>();
        }
    }
}
