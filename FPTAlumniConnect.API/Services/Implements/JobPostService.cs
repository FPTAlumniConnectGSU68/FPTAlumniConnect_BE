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
            _logger.LogInformation("Creating new job post: {JobTitle}", request.JobTitle);

            if (request.MinSalary > request.MaxSalary)
            {
                throw new BadHttpRequestException("MinSalary cannot be greater than MaxSalary");
            }

            JobPost newJobPost = _mapper.Map<JobPost>(request);
            newJobPost.CreatedAt = DateTime.Now;
            //newJobPost.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
            {
                _logger.LogError("Create job post failed.");
                throw new BadHttpRequestException("CreateFailed");
            }

            _logger.LogInformation("Created job post with ID: {Id}", newJobPost.JobPostId);
            return newJobPost.JobPostId;
        }

        public async Task<JobPostResponse> GetJobPostById(int id)
        {
            _logger.LogInformation("Retrieving job post by ID: {Id}", id);

            Func<IQueryable<JobPost>, IIncludableQueryable<JobPost, object>> include = q => q.Include(u => u.Major);
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id), include: include) ??
                throw new BadHttpRequestException("JobPostNotFound");

            JobPostResponse result = _mapper.Map<JobPostResponse>(jobPost);
            return result;
        }

        public async Task<bool> UpdateJobPostInfo(int id, JobPostInfo request)
        {
            _logger.LogInformation("Updating job post with ID: {Id}", id);

            if (request.MinSalary > request.MaxSalary)
            {
                throw new BadHttpRequestException("MinSalary cannot be greater than MaxSalary");
            }

            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            UpdateJobPostFields(jobPost, request);

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (isSuccessful)
                _logger.LogInformation("Successfully updated job post ID: {Id}", id);
            else
                _logger.LogWarning("Update job post failed for ID: {Id}", id);

            return isSuccessful;
        }

        private void UpdateJobPostFields(JobPost jobPost, JobPostInfo request)
        {
            if (request.MinSalary > request.MaxSalary)
            {
                throw new BadHttpRequestException("MinSalary cannot be greater than MaxSalary");
            }

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
        }

        // NOT YET
        public async Task<bool> DeleteJobPost(int id)
        {
            _logger.LogInformation("Soft deleting job post with ID: {Id}", id);

            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == id /*&& !x.IsDeleted*/) ??
                throw new BadHttpRequestException("JobPostNotFound");

            //jobPost.IsDeleted = true;
            jobPost.UpdatedAt = DateTime.Now;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            //_logger.LogInformation("Soft delete {(isSuccessful ? "succeeded" : "failed")} for ID: {Id}", id);
            return isSuccessful;
        }

        public async Task<IEnumerable<JobPostResponse>> SearchJobPosts(string keyword)
        {
            _logger.LogInformation("Searching job posts with keyword: {Keyword}", keyword);

            var jobPosts = await _unitOfWork.GetRepository<JobPost>().GetListAsync(
                predicate: x => /*!x.IsDeleted &&*/
                                (x.JobTitle.Contains(keyword) || x.JobDescription.Contains(keyword)),
                selector: x => _mapper.Map<JobPostResponse>(x));
                //,: DefaultIncludes());

            return jobPosts;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all job posts with paging: Page {Page}, Size {Size}", pagingModel.page, pagingModel.size);

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