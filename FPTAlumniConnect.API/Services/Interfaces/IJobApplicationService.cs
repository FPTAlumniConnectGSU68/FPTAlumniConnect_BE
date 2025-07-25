﻿using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IJobApplicationService
    {
        Task<int> CreateNewJobApplication(JobApplicationInfo request);
        Task<IPaginate<JobApplicationResponse>> ViewAllJobApplications(JobApplicationFilter filter, PagingModel pagingModel);
        Task<bool> UpdateJobApplicationInfo(int id, JobApplicationInfo request);
        Task<JobApplicationResponse> GetJobApplicationById(int id);

        Task<int> CountAllJobApplications();
        Task<List<JobApplicationResponse>> GetJobApplicationsByCvId(int cvId);
        Task<List<JobApplicationResponse>> GetJobApplicationsByJobPostId(int jobPostId);
        //Task<bool> DeleteJobApplication(int id);
        Task<bool> HasAlreadyApplied(int jobPostId, int cvId);
    }
}
