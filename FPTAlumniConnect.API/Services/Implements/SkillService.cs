using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class SkillService : BaseService<SkillService>, ISkillService
    {
        public SkillService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SkillService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Create a new skill and assign it to a CV
        public async Task<int> CreateNewSkill(SkillJobInfo request)
        {
            var cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.CvID,
                orderBy: null,
                include: null
            ) ?? throw new BadHttpRequestException("CvIdNotFound");

            var skillRepo = _unitOfWork.GetRepository<Skill>();
            var existingSkill = await skillRepo.SingleOrDefaultAsync(
                predicate: s => s.Name == request.Skill,
                orderBy: null,
                include: null
            );

            if (existingSkill == null)
            {
                existingSkill = new Skill
                {
                    Name = request.Skill!,
                    CreatedAt = DateTime.Now,
                };
                await skillRepo.InsertAsync(existingSkill);
            }

            var cvSkillRepo = _unitOfWork.GetRepository<CvSkill>();
            var duplicate = await cvSkillRepo.SingleOrDefaultAsync(
                predicate: x => x.CvId == request.CvID && x.SkillId == existingSkill.SkillId,
                orderBy: null,
                include: null
            );

            if (duplicate != null)
            {
                throw new BadHttpRequestException("Skill already assigned to CV.");
            }

            var cvSkill = new CvSkill
            {
                CvId = request.CvID!.Value,
                SkillId = existingSkill.SkillId,
                CreatedAt = DateTime.Now,
            };

            await cvSkillRepo.InsertAsync(cvSkill);

            bool success = await _unitOfWork.CommitAsync() > 0;
            if (!success) throw new BadHttpRequestException("CreateFailed");

            return existingSkill.SkillId;
        }

        // Get all skill names assigned to a specific CV
        public async Task<List<string>> GetSkillsByCvId(int cvId)
        {
            var skills = await _unitOfWork.GetRepository<CvSkill>().GetListAsync(
                predicate: x => x.CvId == cvId,
                include: x => x.Include(cs => cs.Skill),
                orderBy: x => x.OrderBy(cs => cs.CreatedAt));

            return skills.Select(cs => cs.Skill.Name).ToList();
        }

        // Delete a skill from a specific CV
        public async Task<bool> DeleteSkillFromCv(int skillId, int cvId)
        {
            var cvSkill = await _unitOfWork.GetRepository<CvSkill>().SingleOrDefaultAsync(
                predicate: x => x.CvId == cvId && x.SkillId == skillId,
                orderBy: null,
                include: null
            ) ?? throw new BadHttpRequestException("NotFound");

            _unitOfWork.GetRepository<CvSkill>().DeleteAsync(cvSkill);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Count number of skills assigned to a CV
        public async Task<int> CountSkillByCvId(int cvId)
        {
            return await _unitOfWork.GetRepository<CvSkill>().CountAsync(x => x.CvId == cvId);
        }
    }
}
