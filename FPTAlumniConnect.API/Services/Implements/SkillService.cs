using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class SkillService : BaseService<SkillService>, ISkillService
    {
        public SkillService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SkillService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewSkill(SkillJobInfo request)
        {
            // Check if CV exists
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            // Check for duplicate skill in the same CV
            SkillJob existingSkillJob = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: s => s.Skill == request.Skill && s.CvID == request.CvID);
            if (existingSkillJob != null)
            {
                throw new BadHttpRequestException("Skill already exists.");
            }

            // Map and insert new skill
            SkillJob newSkill = _mapper.Map<SkillJob>(request);
            await _unitOfWork.GetRepository<SkillJob>().InsertAsync(newSkill);

            // Commit to DB
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newSkill.SkillJobId;
        }

        public async Task<SkillJobReponse> GetSkillById(int id)
        {
            // Get skill by ID
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.SkillJobId.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            return _mapper.Map<SkillJobReponse>(skill);
        }

        public async Task<SkillJobReponse> GetSkillByCvId(int id)
        {
            // Get a skill by CV ID (returns only one)
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.CvID.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            return _mapper.Map<SkillJobReponse>(skill);
        }

        public async Task<List<SkillJobReponse>> GetSkillsByCvId(int cvId)
        {
            // Get all skills by CV ID
            var skills = await _unitOfWork.GetRepository<SkillJob>().GetListAsync(
                predicate: x => x.CvID == cvId,
                orderBy: q => q.OrderBy(x => x.CreatedAt));

            return skills.Select(s => _mapper.Map<SkillJobReponse>(s)).ToList();
        }

        public async Task<bool> UpdateSkillInfo(int id, SkillJobInfo request)
        {
            // Find skill by ID
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.SkillJobId.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            // Check if CV exists
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            // Check for duplicate skill
            if (!string.IsNullOrEmpty(request.Skill))
            {
                var duplicate = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                    predicate: s => s.Skill == request.Skill && s.CvID == request.CvID && s.SkillJobId != id);
                if (duplicate != null)
                {
                    throw new BadHttpRequestException("Skill already exists.");
                }

                skill.Skill = request.Skill;
            }

            // Update metadata
            skill.Skill = string.IsNullOrEmpty(request.Skill) ? skill.Skill : request.Skill;
            skill.UpdatedAt = DateTime.Now;
            skill.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<SkillJob>().UpdateAsync(skill);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<SkillJobReponse>> ViewAllSkill(SkillJobFilter filter, PagingModel pagingModel)
        {
            // Get all skills with filtering and pagination
            IPaginate<SkillJobReponse> response = await _unitOfWork.GetRepository<SkillJob>().GetPagingListAsync(
                selector: x => _mapper.Map<SkillJobReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);
            return response;
        }

        public async Task<bool> DeleteSkill(int id)
        {
            // Find and delete skill by ID
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.SkillJobId.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            _unitOfWork.GetRepository<SkillJob>().DeleteAsync(skill);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<int> CountSkillByCvId(int cvId)
        {
            // Count skills by CV ID
            return await _unitOfWork.GetRepository<SkillJob>().CountAsync(x => x.CvID == cvId);
        }
    }
}
