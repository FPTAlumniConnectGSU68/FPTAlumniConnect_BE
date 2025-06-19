using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IMentorshipService 
    {
        Task<int> CreateNewMentorship(MentorshipInfo request);
        Task<IPaginate<MentorshipReponse>> ViewAllMentorship(MentorshipFilter filter, PagingModel pagingModel);
        Task<bool> UpdateMentorshipInfo(int id, MentorshipInfo request);
        Task<MentorshipReponse> GetMentorshipById(int id);
    }
}
