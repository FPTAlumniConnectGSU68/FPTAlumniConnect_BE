using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobPostService : BaseService<JobPostService>, IJobPostService
    {
        public JobPostService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<JobPostService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewJobPost(JobPostInfo request)
        {
            JobPost newJobPost = _mapper.Map<JobPost>(request);
            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJobPost.JobPostId;
        }

        public async Task<JobPostResponse> GetJobPostById(int id)
        {
            Func<IQueryable<JobPost>, IIncludableQueryable<JobPost, object>> include = q => q.Include(u => u.Major);
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id), include: include) ??
                throw new BadHttpRequestException("JobPostNotFound");

            JobPostResponse result = _mapper.Map<JobPostResponse>(jobPost);
            return result;
        }

        public async Task<bool> UpdateJobPostInfo(int id, JobPostInfo request)
        {
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            jobPost.JobDescription = string.IsNullOrEmpty(request.JobDescription) ? jobPost.JobDescription : request.JobDescription;
            jobPost.Requirements = string.IsNullOrEmpty(request.Requirements) ? jobPost.Requirements : request.Requirements;
            jobPost.Location = string.IsNullOrEmpty(request.Location) ? jobPost.Location : request.Location;
            jobPost.Benefits = string.IsNullOrEmpty(request.Benefits) ? jobPost.Benefits : request.Benefits;
            jobPost.JobTitle = string.IsNullOrEmpty(request.JobTitle) ? jobPost.JobTitle : request.JobTitle;
            jobPost.MinSalary = request.MinSalary;
            jobPost.MaxSalary = request.MaxSalary;
            jobPost.Time = request.Time;
            jobPost.Status = request.Status;
            jobPost.Email = string.IsNullOrEmpty(request.Email) ? jobPost.Email : request.Email;
            jobPost.MajorId = request.MajorId;
            jobPost.UpdatedAt = DateTime.Now;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            Func<IQueryable<JobPost>, IIncludableQueryable<JobPost, object>> include = q => q.Include(u => u.Major);
            IPaginate<JobPostResponse> response = await _unitOfWork.GetRepository<JobPost>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostResponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }

}