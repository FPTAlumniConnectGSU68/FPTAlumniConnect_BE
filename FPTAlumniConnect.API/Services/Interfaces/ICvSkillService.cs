using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CvSkill;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ICvSkillService
    {
        Task<int> CreateCvSkill(CvSkillInfo request);
        Task<CvSkillResponse> GetCvSkillById(int cvId, int skillId);
        Task<bool> UpdateCvSkill(int cvId, int skillId, CvSkillInfo request);
        Task<IPaginate<CvSkillResponse>> ViewAllCvSkills(CvSkillFilter filter, PagingModel pagingModel);
        Task<bool> DeleteCvSkill(int cvId, int skillId);
    }
}