using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPostSkill;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IJobPostSkillService
    {
        Task<int> CreateJobPostSkill(JobPostSkillInfo request);
        Task<JobPostSkillResponse> GetJobPostSkillById(int jobPostId, int skillId);
        Task<bool> UpdateJobPostSkill(int jobPostId, int skillId, JobPostSkillInfo request);
        Task<IPaginate<JobPostSkillResponse>> ViewAllJobPostSkills(JobPostSkillFilter filter, PagingModel pagingModel);
        Task<bool> DeleteJobPostSkill(int jobPostId, int skillId);
    }
}