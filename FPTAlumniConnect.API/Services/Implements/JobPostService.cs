using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
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

        public async Task<int> CreateNewJobPost(int idUser, JobPostInfo request)
        {
            _logger.LogInformation("Creating new job post: {JobTitle} for user ID: {UserId}", request.JobTitle, idUser);

            // Validate user ID
            if (idUser <= 0)
                throw new BadHttpRequestException("Invalid user ID. User ID must be greater than 0.");

            // Check if user exists and has active recruiter status
            var user = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(
                    predicate: x => x.UserId == idUser,
                    include: q => q.Include(u => u.RecruiterInfos))
                ?? throw new BadHttpRequestException("User not found.");

            if (user.RecruiterInfos == null || user.RecruiterInfos.Status == null)
                throw new BadHttpRequestException("User is not an active recruiter.");

            // Validate salary
            ValidateSalary(request.MinSalary, request.MaxSalary);

            // Map JobPostInfo to JobPost and set UserId
            JobPost newJobPost = _mapper.Map<JobPost>(request);
            newJobPost.UserId = idUser;
            newJobPost.CreatedAt = TimeHelper.NowInVietnam();
            newJobPost.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Insert job post
            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("Failed to create job post.");

            // Add associated skills
            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    var jobPostSkill = new JobPostSkill
                    {
                        JobPostId = newJobPost.JobPostId,
                        SkillId = skillId,
                        CreatedAt = TimeHelper.NowInVietnam(),
                        UpdatedAt = TimeHelper.NowInVietnam()
                    };
                    await _unitOfWork.GetRepository<JobPostSkill>().InsertAsync(jobPostSkill);
                }
                isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful) throw new BadHttpRequestException("Failed to add job post skills.");
            }

            return newJobPost.JobPostId;
        }

        public async Task<JobPostResponse> GetJobPostById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID. ID must be greater than 0.", nameof(id));

            _logger.LogInformation("Retrieving job post by ID: {Id}", id);

            var jobPost = await _unitOfWork.GetRepository<JobPost>()
                .SingleOrDefaultAsync(
                    predicate: x => x.JobPostId == id,
                    include: q => q.Include(j => j.User).ThenInclude(u => u.RecruiterInfos)
                                  .Include(j => j.Major)
                                  .Include(j => j.JobPostSkills).ThenInclude(jps => jps.Skill))
                ?? throw new BadHttpRequestException("JobPostNotFound");

            return _mapper.Map<JobPostResponse>(jobPost);
        }

        public async Task<bool> UpdateJobPostInfo(int id, JobPostInfo request)
        {
            ValidateSalary(request.MinSalary, request.MaxSalary);

            var jobPost = await _unitOfWork.GetRepository<JobPost>()
                .SingleOrDefaultAsync(
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
            jobPost.UpdatedAt = TimeHelper.NowInVietnam();
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
                        CreatedAt = TimeHelper.NowInVietnam(),
                        UpdatedAt = TimeHelper.NowInVietnam()
                    };
                    await _unitOfWork.GetRepository<JobPostSkill>().InsertAsync(jobPostSkill);
                }
            }

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteJobPost(int id)
        {
            var jobPost = await _unitOfWork.GetRepository<JobPost>()
                .SingleOrDefaultAsync(
                    predicate: x => x.JobPostId == id)
                ?? throw new BadHttpRequestException("JobPostNotFound");

            jobPost.UpdatedAt = TimeHelper.NowInVietnam();
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
                    include: q => q.Include(j => j.User).ThenInclude(u => u.RecruiterInfos)
                                  .Include(j => j.Major)
                                  .Include(j => j.JobPostSkills).ThenInclude(jps => jps.Skill),
                    orderBy: x => x.OrderByDescending(x => x.CreatedAt));

            return jobPosts;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all job posts with filter and paging.");

            Func<IQueryable<JobPost>, IIncludableQueryable<JobPost, object>> include =
                q => q.Include(j => j.User).ThenInclude(u => u.RecruiterInfos)
                      .Include(j => j.Major)
                      .Include(j => j.JobPostSkills).ThenInclude(jps => jps.Skill);

            IPaginate<JobPostResponse> response = await _unitOfWork.GetRepository<JobPost>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostResponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);

            return response;
        }

        public async Task<int> CountAllJobPosts()
        {
            _logger.LogInformation("Counting all job posts.");

            int count = await _unitOfWork.GetRepository<JobPost>().CountAsync(predicate: x => true);
            return count;
        }

        public async Task<ICollection<CountByMonthResponse>> CountJobPostsByMonth(int? month, int? year)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByMonthResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<JobPost>().GetListAsync(
                    selector: x => _mapper.Map<JobPostResponse>(x),
                    predicate: x => x.CreatedAt.HasValue
                                    && x.CreatedAt.Value.Year == targetYear
                                    && x.CreatedAt.Value.Month == m
                );
                result.Add(new CountByMonthResponse
                {
                    Month = m,
                    Year = targetYear,
                    Count = users.Count()
                });
            }
            return result;
        }

        private void ValidateSalary(int? minSalary, int? maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new BadHttpRequestException("Salary values must be positive numbers");
            if (minSalary > maxSalary)
                throw new BadHttpRequestException("Minimum salary cannot be greater than maximum salary");
            if (maxSalary > 1000000000000)
                throw new BadHttpRequestException("Maximum salary exceeds reasonable limit");
        }

        public async Task<IEnumerable<JobPostResponse>> GetJobPostsByStatus(string status)
        {
            var jobPosts = await _unitOfWork.GetRepository<JobPost>().GetListAsync(
                predicate: x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase),
                selector: x => _mapper.Map<JobPostResponse>(x),
                include: q => q.Include(j => j.User).ThenInclude(u => u.RecruiterInfos)
                              .Include(j => j.Major)
                              .Include(j => j.JobPostSkills).ThenInclude(jps => jps.Skill));

            return jobPosts;
        }
    }
}