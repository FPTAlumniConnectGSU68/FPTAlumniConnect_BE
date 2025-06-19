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
            // Check CvId
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
            predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            SkillJob existingSkillJob = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
            predicate: s => s.Skill == request.Skill && s.CvID == request.CvID);
            if (existingSkillJob != null)
            {
                throw new BadHttpRequestException("Skill already exists.");
            }

            SkillJob newSkill = _mapper.Map<SkillJob>(request);

            await _unitOfWork.GetRepository<SkillJob>().InsertAsync(newSkill);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newSkill.SkillJobId;
        }

        public async Task<SkillJobReponse> GetSkillById(int id)
        {
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.SkillJobId.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            SkillJobReponse result = _mapper.Map<SkillJobReponse>(skill);
            return result;
        }

        public async Task<SkillJobReponse> GetSkillByCvId(int id)
        {
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.CvID.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            SkillJobReponse result = _mapper.Map<SkillJobReponse>(skill);
            return result;
        }

        public async Task<bool> UpdateSkillInfo(int id, SkillJobInfo request)
        {
            SkillJob skill = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
                predicate: x => x.SkillJobId.Equals(id)) ??
                throw new BadHttpRequestException("SkillNotFound");

            // Check CvId
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
            predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            SkillJob existingSkillJob = await _unitOfWork.GetRepository<SkillJob>().SingleOrDefaultAsync(
            predicate: s => s.Skill == request.Skill && s.CvID == request.CvID);
            if (existingSkillJob != null)
            {
                throw new BadHttpRequestException("Skill already exists.");
            }

            skill.Skill = string.IsNullOrEmpty(request.Skill) ? skill.Skill : request.Skill;
            skill.UpdatedAt = DateTime.Now;
            skill.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<SkillJob>().UpdateAsync(skill);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<SkillJobReponse>> ViewAllSkill(SkillJobFilter filter, PagingModel pagingModel)
        {
            IPaginate<SkillJobReponse> response = await _unitOfWork.GetRepository<SkillJob>().GetPagingListAsync(
                selector: x => _mapper.Map<SkillJobReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
