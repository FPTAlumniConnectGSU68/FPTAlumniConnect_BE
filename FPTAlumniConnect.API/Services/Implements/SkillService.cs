using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class SkillService : BaseService<SkillService>, ISkillService
    {
        public SkillService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SkillService> logger,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateSkill(SkillInfo request)
        {
            _logger.LogInformation("Creating new Skill with Name: {Name}", request.Name);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadHttpRequestException("Skill name cannot be empty");

            var exists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.Name == request.Name);
            if (exists)
                throw new BadHttpRequestException("Skill with this name already exists");

            Skill newSkill = _mapper.Map<Skill>(request);
            newSkill.CreatedAt = TimeHelper.NowInVietnam();
            newSkill.UpdatedAt = TimeHelper.NowInVietnam();

            await _unitOfWork.GetRepository<Skill>().InsertAsync(newSkill);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            _logger.LogInformation("Skill created successfully with SkillId: {SkillId}, Name: {Name}",
                newSkill.SkillId, newSkill.Name);
            return newSkill.SkillId;
        }

        public async Task<SkillResponse> GetSkillById(int skillId)
        {
            _logger.LogInformation("Getting Skill by SkillId: {SkillId}", skillId);

            Skill skill = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                predicate: x => x.SkillId == skillId) ??
                throw new BadHttpRequestException("SkillNotFound");

            SkillResponse result = _mapper.Map<SkillResponse>(skill);
            return result;
        }

        public async Task<bool> UpdateSkill(int skillId, SkillInfo request)
        {
            _logger.LogInformation("Updating Skill with SkillId: {SkillId}", skillId);

            Skill skill = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                predicate: x => x.SkillId == skillId) ??
                throw new BadHttpRequestException("SkillNotFound");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadHttpRequestException("Skill name cannot be empty");

            if (skill.Name != request.Name)
            {
                var exists = await _unitOfWork.GetRepository<Skill>().AnyAsync(x => x.Name == request.Name);
                if (exists)
                    throw new BadHttpRequestException("Skill with this name already exists");
            }

            _mapper.Map(request, skill);
            skill.UpdatedAt = TimeHelper.NowInVietnam();

            _unitOfWork.GetRepository<Skill>().UpdateAsync(skill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("Skill updated successfully: SkillId: {SkillId}, Name: {Name}", skillId, skill.Name);
            else
                _logger.LogWarning("Skill update failed: SkillId: {SkillId}", skillId);

            return isSuccessful;
        }

        public async Task<IPaginate<SkillResponse>> ViewAllSkills(SkillFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all Skills with paging: Page {Page}, Size {Size}",
                pagingModel.page, pagingModel.size);

            IPaginate<SkillResponse> response = await _unitOfWork.GetRepository<Skill>().GetPagingListAsync(
                selector: x => _mapper.Map<SkillResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteSkill(int skillId)
        {
            _logger.LogInformation("Deleting Skill with SkillId: {SkillId}", skillId);

            Skill skill = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                predicate: x => x.SkillId == skillId) ??
                throw new BadHttpRequestException("SkillNotFound");

            var hasAssociations = await _unitOfWork.GetRepository<CvSkill>().AnyAsync(x => x.SkillId == skillId) ||
                                 await _unitOfWork.GetRepository<JobPostSkill>().AnyAsync(x => x.SkillId == skillId);
            if (hasAssociations)
                throw new BadHttpRequestException("Cannot delete Skill because it is associated with CVs or JobPosts");

            _unitOfWork.GetRepository<Skill>().DeleteAsync(skill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("Skill deleted successfully: SkillId: {SkillId}", skillId);
            else
                _logger.LogWarning("Skill deletion failed: SkillId: {SkillId}", skillId);

            return isSuccessful;
        }
    }
}