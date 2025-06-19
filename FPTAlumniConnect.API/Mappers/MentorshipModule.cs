using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class MentorshipModule : Profile
    {
        public MentorshipModule()
        {
            CreateMap<Mentorship, MentorshipReponse>();
            CreateMap<MentorshipInfo, Mentorship>();
        }
    }
}
