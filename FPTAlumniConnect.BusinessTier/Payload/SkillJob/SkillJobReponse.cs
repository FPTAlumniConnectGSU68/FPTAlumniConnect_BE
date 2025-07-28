namespace FPTAlumniConnect.BusinessTier.Payload.SkillJob
{
    public class SkillResponse
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
