using FPTAlumniConnect.BusinessTier.Payload.SkillJob;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ISkillService
    {
        // Create or reuse a skill and assign it to a CV
        Task<int> CreateNewSkill(SkillJobInfo request);

        // Get all skill names assigned to a CV
        Task<List<string>> GetSkillsByCvId(int cvId);

        // Delete a skill from a specific CV
        Task<bool> DeleteSkillFromCv(int skillId, int cvId);

        // Count number of skills assigned to a CV
        Task<int> CountSkillByCvId(int cvId);
    }
}
