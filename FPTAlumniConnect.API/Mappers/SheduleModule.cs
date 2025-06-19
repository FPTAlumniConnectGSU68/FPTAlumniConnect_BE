using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class ScheduleModule : Profile
    {
        public ScheduleModule()
        {
            CreateMap<Schedule, ScheduleReponse>();
            CreateMap<ScheduleInfo, Schedule>();
        }
    }
}
