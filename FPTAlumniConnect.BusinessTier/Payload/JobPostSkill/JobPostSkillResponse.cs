namespace FPTAlumniConnect.BusinessTier.Payload.JobPostSkill
{
    public class JobPostSkillResponse
    {
        public int JobPostId { get; set; }
        public int SkillId { get; set; }
        public string? JobPostTitle { get; set; }
        public string? SkillName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
