using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ISkillService
    {
        Task<int> CreateSkill(SkillInfo request);
        Task<SkillResponse> GetSkillById(int skillId);
        Task<bool> UpdateSkill(int skillId, SkillInfo request);
        Task<IPaginate<SkillResponse>> ViewAllSkills(SkillFilter filter, PagingModel pagingModel);
        Task<bool> DeleteSkill(int skillId);
    }
}