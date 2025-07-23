using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPostSkill;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobPostSkillService : BaseService<JobPostSkillService>, IJobPostSkillService
    {
        public JobPostSkillService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<JobPostSkillService> logger,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateJobPostSkill(JobPostSkillInfo request)
        {
            _logger.LogInformation("Creating new JobPost-Skill association for JobPostId: {JobPostId}, SkillId: {SkillId}",
                request.JobPostId, request.SkillId);

            if (request.JobPostId <= 0 || request.SkillId <= 0)
                throw new BadHttpRequestException("JobPostId and SkillId must be valid positive integers");

            var jobPostExists = await _unitOfWork.GetRepository<JobPost>().AnyAsync(x => x.JobPostId == request.JobPostId);
            var skillExists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.SkillId == request.SkillId);
            if (!jobPostExists || !skillExists)
                throw new BadHttpRequestException("JobPost or Skill not found");

            var exists = await _unitOfWork.GetRepository<JobPostSkill>().AnyAsync(
                x => x.JobPostId == request.JobPostId && x.SkillId == request.SkillId);
            if (exists)
                throw new BadHttpRequestException("JobPost-Skill association already exists");

            JobPostSkill newJobPostSkill = _mapper.Map<JobPostSkill>(request);
            newJobPostSkill.CreatedAt = DateTime.Now;
            newJobPostSkill.UpdatedAt = DateTime.Now;

            await _unitOfWork.GetRepository<JobPostSkill>().InsertAsync(newJobPostSkill);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            _logger.LogInformation("JobPost-Skill created successfully for JobPostId: {JobPostId}, SkillId: {SkillId}",
                newJobPostSkill.JobPostId, newJobPostSkill.SkillId);
            return newJobPostSkill.JobPostId;
        }

        public async Task<JobPostSkillResponse> GetJobPostSkillById(int jobPostId, int skillId)
        {
            _logger.LogInformation("Getting JobPost-Skill by JobPostId: {JobPostId}, SkillId: {SkillId}", jobPostId, skillId);

            Func<IQueryable<JobPostSkill>, IIncludableQueryable<JobPostSkill, object>> include =
                q => q.Include(x => x.JobPost).Include(x => x.Skill);

            JobPostSkill jobPostSkill = await _unitOfWork.GetRepository<JobPostSkill>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == jobPostId && x.SkillId == skillId,
                include: include) ??
                throw new BadHttpRequestException("JobPostSkillNotFound");

            JobPostSkillResponse result = _mapper.Map<JobPostSkillResponse>(jobPostSkill);
            result.JobPostTitle = jobPostSkill.JobPost?.JobTitle;
            result.SkillName = jobPostSkill.Skill?.Name;
            return result;
        }

        public async Task<bool> UpdateJobPostSkill(int jobPostId, int skillId, JobPostSkillInfo request)
        {
            _logger.LogInformation("Updating JobPost-Skill with JobPostId: {JobPostId}, SkillId: {SkillId}", jobPostId, skillId);

            JobPostSkill jobPostSkill = await _unitOfWork.GetRepository<JobPostSkill>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == jobPostId && x.SkillId == skillId) ??
                throw new BadHttpRequestException("JobPostSkillNotFound");

            var jobPostExists = await _unitOfWork.GetRepository<JobPost>().AnyAsync(x => x.JobPostId == request.JobPostId);
            var skillExists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.SkillId == request.SkillId);
            if (!jobPostExists || !skillExists)
                throw new BadHttpRequestException("JobPost or Skill not found");

            if (jobPostSkill.JobPostId != request.JobPostId || jobPostSkill.SkillId != request.SkillId)
            {
                var exists = await _unitOfWork.GetRepository<JobPostSkill>().AnyAsync(
                    x => x.JobPostId == request.JobPostId && x.SkillId == request.SkillId);
                if (exists)
                    throw new BadHttpRequestException("New JobPost-Skill association already exists");
            }

            _mapper.Map(request, jobPostSkill);
            jobPostSkill.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<JobPostSkill>().UpdateAsync(jobPostSkill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("JobPost-Skill updated successfully: JobPostId: {JobPostId}, SkillId: {SkillId}",
                    jobPostId, skillId);
            else
                _logger.LogWarning("JobPost-Skill update failed: JobPostId: {JobPostId}, SkillId: {SkillId}",
                    jobPostId, skillId);

            return isSuccessful;
        }

        public async Task<IPaginate<JobPostSkillResponse>> ViewAllJobPostSkills(JobPostSkillFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all JobPost-Skills with paging: Page {Page}, Size {Size}",
                pagingModel.page, pagingModel.size);

            Func<IQueryable<JobPostSkill>, IIncludableQueryable<JobPostSkill, object>> include =
                q => q.Include(x => x.JobPost).Include(x => x.Skill);

            IPaginate<JobPostSkillResponse> response = await _unitOfWork.GetRepository<JobPostSkill>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostSkillResponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteJobPostSkill(int jobPostId, int skillId)
        {
            _logger.LogInformation("Deleting JobPost-Skill with JobPostId: {JobPostId}, SkillId: {SkillId}",
                jobPostId, skillId);

            JobPostSkill jobPostSkill = await _unitOfWork.GetRepository<JobPostSkill>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId == jobPostId && x.SkillId == skillId) ??
                throw new BadHttpRequestException("JobPostSkillNotFound");

            _unitOfWork.GetRepository<JobPostSkill>().DeleteAsync(jobPostSkill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("JobPost-Skill deleted successfully: JobPostId: {JobPostId}, SkillId: {SkillId}",
                    jobPostId, skillId);
            else
                _logger.LogWarning("JobPost-Skill deletion failed: JobPostId: {JobPostId}, SkillId: {SkillId}",
                    jobPostId, skillId);

            return isSuccessful;
        }

        private Func<IQueryable<JobPostSkill>, IIncludableQueryable<JobPostSkill, object>> DefaultIncludes()
        {
            return q => q.Include(x => x.JobPost).Include(x => x.Skill);
        }
    }
}