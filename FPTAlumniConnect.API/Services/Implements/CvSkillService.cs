using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CvSkill;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class CvSkillService : BaseService<CvSkillService>, ICvSkillService
    {
        public CvSkillService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<CvSkillService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateCvSkill(CvSkillInfo request)
        {
            _logger.LogInformation("Creating new CV-Skill association for CvId: {CvId}, SkillId: {SkillId}",
                request.CvId, request.SkillId);

            // Validate input
            if (request.CvId <= 0 || request.SkillId <= 0)
                throw new BadHttpRequestException("CvId and SkillId must be valid positive integers");

            // Check if CV and Skill exist
            var cvExists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == request.CvId);
            var skillExists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.SkillId == request.SkillId);
            if (!cvExists || !skillExists)
                throw new BadHttpRequestException("CV or Skill not found");

            // Check if association already exists
            var exists = await _unitOfWork.GetRepository<CvSkill>().AnyAsync(
                x => x.CvId == request.CvId && x.SkillId == request.SkillId);
            if (exists)
                throw new BadHttpRequestException("CV-Skill association already exists");

            CvSkill newCvSkill = _mapper.Map<CvSkill>(request);
            newCvSkill.CreatedAt = DateTime.Now;
            newCvSkill.UpdatedAt = DateTime.Now;

            await _unitOfWork.GetRepository<CvSkill>().InsertAsync(newCvSkill);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            _logger.LogInformation("CV-Skill created successfully for CvId: {CvId}, SkillId: {SkillId}",
                newCvSkill.CvId, newCvSkill.SkillId);
            return newCvSkill.CvId;
        }

        public async Task<CvSkillResponse> GetCvSkillById(int cvId, int skillId)
        {
            _logger.LogInformation("Getting CV-Skill by CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);

            Func<IQueryable<CvSkill>, IIncludableQueryable<CvSkill, object>> include =
                q => q.Include(x => x.Cv).Include(x => x.Skill);

            CvSkill cvSkill = await _unitOfWork.GetRepository<CvSkill>().SingleOrDefaultAsync(
                predicate: x => x.CvId == cvId && x.SkillId == skillId,
                include: include) ??
                throw new BadHttpRequestException("CvSkillNotFound");

            CvSkillResponse result = _mapper.Map<CvSkillResponse>(cvSkill);
            result.CvTitle = cvSkill.Cv?.FullName; // Corrected to use Title property
            result.SkillName = cvSkill.Skill?.Name;
            return result;
        }

        public async Task<bool> UpdateCvSkill(int cvId, int skillId, CvSkillInfo request)
        {
            _logger.LogInformation("Updating CV-Skill with CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);

            CvSkill cvSkill = await _unitOfWork.GetRepository<CvSkill>().SingleOrDefaultAsync(
                predicate: x => x.CvId == cvId && x.SkillId == skillId) ??
                throw new BadHttpRequestException("CvSkillNotFound");

            // Validate new CV and Skill exist
            var cvExists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == request.CvId);
            var skillExists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.SkillId == request.SkillId);
            if (!cvExists || !skillExists)
                throw new BadHttpRequestException("CV or Skill not found");

            // Check if new association already exists (if changing CvId or SkillId)
            if (cvSkill.CvId != request.CvId || cvSkill.SkillId != request.SkillId)
            {
                var exists = await _unitOfWork.GetRepository<CvSkill>().AnyAsync(
                    x => x.CvId == request.CvId && x.SkillId == request.SkillId);
                if (exists)
                    throw new BadHttpRequestException("New CV-Skill association already exists");
            }

            _mapper.Map(request, cvSkill);
            cvSkill.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<CvSkill>().UpdateAsync(cvSkill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("CV-Skill updated successfully: CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);
            else
                _logger.LogWarning("CV-Skill update failed: CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);

            return isSuccessful;
        }

        public async Task<IPaginate<CvSkillResponse>> ViewAllCvSkills(CvSkillFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all CV-Skills with paging: Page {Page}, Size {Size}",
                pagingModel.page, pagingModel.size);

            Func<IQueryable<CvSkill>, IIncludableQueryable<CvSkill, object>> include =
                q => q.Include(x => x.Cv).Include(x => x.Skill);

            IPaginate<CvSkillResponse> response = await _unitOfWork.GetRepository<CvSkill>().GetPagingListAsync(
                selector: x => _mapper.Map<CvSkillResponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteCvSkill(int cvId, int skillId)
        {
            _logger.LogInformation("Deleting CV-Skill with CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);

            CvSkill cvSkill = await _unitOfWork.GetRepository<CvSkill>().SingleOrDefaultAsync(
                predicate: x => x.CvId == cvId && x.SkillId == skillId) ??
                throw new BadHttpRequestException("CvSkillNotFound");

            _unitOfWork.GetRepository<CvSkill>().DeleteAsync(cvSkill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("CV-Skill deleted successfully: CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);
            else
                _logger.LogWarning("CV-Skill deletion failed: CvId: {CvId}, SkillId: {SkillId}", cvId, skillId);

            return isSuccessful;
        }

        private Func<IQueryable<CvSkill>, IIncludableQueryable<CvSkill, object>> DefaultIncludes()
        {
            return q => q.Include(x => x.Cv).Include(x => x.Skill);
        }
    }
}