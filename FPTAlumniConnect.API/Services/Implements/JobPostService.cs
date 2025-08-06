using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            newJobPost.CreatedAt = DateTime.UtcNow;
            newJobPost.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    var jobPostSkill = new JobPostSkill
                    {
                        JobPostId = newJobPost.JobPostId,
                        SkillId = skillId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<JobPostSkill>().InsertAsync(jobPostSkill);
                }
                isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful) throw new BadHttpRequestException("Failed to add JobPost skills");
            }

            return newJobPost.JobPostId;
        }

        public async Task<JobPostResponse> GetJobPostById(int id)
        {
            var jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == id,
                include: q => q.Include(x => x.Major).Include(x => x.JobPostSkills).ThenInclude(jps => jps.Skill))
                ?? throw new BadHttpRequestException("JobPostNotFound");

            return _mapper.Map<JobPostResponse>(jobPost);
        }

        public async Task<bool> UpdateJobPostInfo(int id, JobPostInfo request)
        {
            ValidateSalary(request.MinSalary, request.MaxSalary);

            var jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == id,
                include: q => q.Include(j => j.JobPostSkills))
                ?? throw new BadHttpRequestException("JobPostNotFound");

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
            jobPost.UpdatedAt = DateTime.UtcNow;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            if (request.SkillIds != null)
            {
                var existingSkills = await _unitOfWork.GetRepository<JobPostSkill>().GetListAsync(
                    predicate: x => x.JobPostId == id);

                foreach (var skill in existingSkills)
                    _unitOfWork.GetRepository<JobPostSkill>().DeleteAsync(skill);

                foreach (var skillId in request.SkillIds)
                {
                    var jobPostSkill = new JobPostSkill
                    {
                        JobPostId = id,
                        SkillId = skillId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<JobPostSkill>().InsertAsync(jobPostSkill);
                }
            }

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteJobPost(int id)
        {
            var jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == id)
                ?? throw new BadHttpRequestException("JobPostNotFound");

            jobPost.UpdatedAt = DateTime.UtcNow;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IEnumerable<JobPostResponse>> SearchJobPosts(string keyword, int? minSalary, int? maxSalary)
        {
            Expression<Func<JobPost, bool>> predicate = x =>
                (string.IsNullOrEmpty(keyword) || x.JobTitle.Contains(keyword) || x.JobDescription.Contains(keyword)) &&
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
                    include: q => q.Include(x => x.JobPostSkills).ThenInclude(jps => jps.Skill),
                    orderBy: x => x.OrderByDescending(x => x.CreatedAt));

            return jobPosts;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all job posts with filter and paging.");

            Func<IQueryable<JobPost>, IIncludableQueryable<JobPost, object>> include =
                q => q.Include(x => x.Major).Include(x => x.JobPostSkills).ThenInclude(jps => jps.Skill);

            IPaginate<JobPostResponse> response = await _unitOfWork.GetRepository<JobPost>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostResponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);

            return response;
        }

        private void ValidateSalary(int? minSalary, int? maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new BadHttpRequestException("Salary values must be positive numbers");
            if (minSalary > maxSalary)
                throw new BadHttpRequestException("Minimum salary cannot be greater than maximum salary");
            if (maxSalary > 1000000)
                throw new BadHttpRequestException("Maximum salary exceeds reasonable limit");
        }

        public async Task<IEnumerable<JobPostResponse>> GetJobPostsByStatus(string status)
        {
            var jobPosts = await _unitOfWork.GetRepository<JobPost>().GetListAsync(
                predicate: x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase),
                selector: x => _mapper.Map<JobPostResponse>(x),
                include: q => q.Include(x => x.JobPostSkills).ThenInclude(jps => jps.Skill));

            return jobPosts;
        }
    }
}
