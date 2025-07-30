using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            ValidateSalary(request.MinSalary, request.MaxSalary);

            JobPost newJobPost = _mapper.Map<JobPost>(request);
            newJobPost.CreatedAt = DateTime.Now;
            newJobPost.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

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

            ValidateSalary(request.MinSalary, request.MaxSalary);

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
            // Validate salary first
            ValidateSalary(request.MinSalary, request.MaxSalary);
            // Update other fields
            jobPost.JobTitle = request.JobTitle ?? jobPost.JobTitle;
            jobPost.JobDescription = request.JobDescription ?? jobPost.JobDescription;
            jobPost.Requirements = request.Requirements ?? jobPost.Requirements;
            jobPost.Location = request.Location ?? jobPost.Location;
            jobPost.Benefits = request.Benefits ?? jobPost.Benefits;
            jobPost.MinSalary = request.MinSalary ?? jobPost.MinSalary;
            jobPost.MaxSalary = request.MaxSalary ?? jobPost.MaxSalary;
            jobPost.Time = request.Time ?? jobPost.Time;
            jobPost.Status = request.Status ?? jobPost.Status;
            jobPost.Email = request.Email ?? jobPost.Email;
            jobPost.MajorId = request.MajorId ?? jobPost.MajorId;
            jobPost.IsDeal = request.IsDeal ?? jobPost.IsDeal;
            jobPost.UpdatedAt = DateTime.Now;

            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            jobPost.UpdatedBy = string.IsNullOrEmpty(userName) ? "system" : userName;
        }

        public async Task<bool> DeleteJobPost(int id)
        {
            _logger.LogInformation("Soft deleting job post with ID: {Id}", id);

            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == id) ??
                throw new BadHttpRequestException("JobPostNotFound");

            jobPost.UpdatedAt = DateTime.Now;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IEnumerable<JobPostResponse>> SearchJobPosts(string keyword, int? minSalary, int? maxSalary)
        {
            _logger.LogInformation("Searching job posts with keyword: {Keyword}", keyword);

            Expression<Func<JobPost, bool>> predicate = x =>
                (string.IsNullOrEmpty(keyword) ||
                 x.JobTitle.Contains(keyword) ||
                 x.JobDescription.Contains(keyword)) &&
                (
                    (!minSalary.HasValue && !maxSalary.HasValue) ||
                    (!minSalary.HasValue && x.MinSalary <= maxSalary) ||
                    (!maxSalary.HasValue && x.MaxSalary >= minSalary) ||
                    (x.MinSalary <= maxSalary && x.MaxSalary >= minSalary)
                );

            var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: predicate,
                    selector: x => _mapper.Map<JobPostResponse>(x),
                    orderBy: x => x.OrderByDescending(x => x.CreatedAt)
                );

            return jobPosts;
        }


        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            var salaryRange = filter.GetSalaryRange();

            Expression<Func<JobPost, bool>> predicate = x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId) &&
                (!filter.MajorId.HasValue || x.MajorId == filter.MajorId) &&
                (
                    // Handle all possible null cases for salary range
                    (!filter.MinSalary.HasValue && !filter.MaxSalary.HasValue) || // No salary filter
                    (!filter.MinSalary.HasValue && x.MinSalary <= filter.MaxSalary) || // Only max filter
                    (!filter.MaxSalary.HasValue && x.MaxSalary >= filter.MinSalary) || // Only min filter
                    (x.MinSalary <= filter.MaxSalary && x.MaxSalary >= filter.MinSalary) // Both min and max
                ) &&
                // Other filters...
                (filter.IsDeal == null || x.IsDeal == filter.IsDeal) &&
                (string.IsNullOrEmpty(filter.Location) || x.Location.Contains(filter.Location)) &&
                (string.IsNullOrEmpty(filter.City) || x.City.Contains(filter.City)) &&
                (string.IsNullOrEmpty(filter.Status) || x.Status.Equals(filter.Status, StringComparison.OrdinalIgnoreCase)) &&
                (!filter.Time.HasValue || x.Time.Date == filter.Time.Value.Date);

            return await _unitOfWork.GetRepository<JobPost>()
                .GetPagingListAsync(
                    selector: x => _mapper.Map<JobPostResponse>(x),
                    predicate: predicate,
                    include: q => q.Include(x => x.Major),
                    orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                    page: pagingModel.page,
                    size: pagingModel.size
                );
        }

        // Phương thức mới để xác thực lương
        private void ValidateSalary(int? minSalary, int? maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
            {
                throw new BadHttpRequestException("Salary values must be positive numbers");
            }

            if (minSalary > maxSalary)
            {
                throw new BadHttpRequestException("Minimum salary cannot be greater than maximum salary");
            }

            if (maxSalary > 1000000) // Ngưỡng lương tối đa hợp lý
            {
                throw new BadHttpRequestException("Maximum salary exceeds reasonable limit");
            }
        }

        // Phương thức mới để lấy danh sách các job post theo trạng thái
        public async Task<IEnumerable<JobPostResponse>> GetJobPostsByStatus(string status)
        {
            _logger.LogInformation("Retrieving job posts with status: {Status}", status);

            var jobPosts = await _unitOfWork.GetRepository<JobPost>().GetListAsync(
                predicate: x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase),
                selector: x => _mapper.Map<JobPostResponse>(x));

            return jobPosts;
        }
    }
}