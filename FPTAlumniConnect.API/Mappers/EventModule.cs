﻿using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class EventModule : Profile
    {
        public EventModule()
        {
            CreateMap<Event, GetEventResponse>();
            CreateMap<EventInfo, Event>();
        }
    }
}
