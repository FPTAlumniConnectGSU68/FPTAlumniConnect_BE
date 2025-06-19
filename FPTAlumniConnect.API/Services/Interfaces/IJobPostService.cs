using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IJobPostService
    {
        Task<int> CreateNewJobPost(JobPostInfo request);
        Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel);
        Task<bool> UpdateJobPostInfo(int id, JobPostInfo request);
        Task<JobPostResponse> GetJobPostById(int id);
    }
}
