using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.WorkExperience;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IWorkExperienceService
    {
        Task<int> CreateWorkExperienceAsync(WorkExperienceInfo request);
        Task<WorkExperienceResponse> GetWorkExperienceByIdAsync(int id);
        Task<bool> UpdateWorkExperienceAsync(int id, WorkExperienceInfo request);
        Task<bool> DeleteWorkExperienceAsync(int id);
        Task<IPaginate<WorkExperienceResponse>> ViewAllWorkExperiencesAsync(WorkExperienceFilter filter, PagingModel pagingModel);
    }
}