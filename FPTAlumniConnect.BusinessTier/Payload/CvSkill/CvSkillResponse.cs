namespace FPTAlumniConnect.BusinessTier.Payload.CvSkill
{
    public class CvSkillResponse
    {
        public int CvId { get; set; }
        public int SkillId { get; set; }
        public string? CvTitle { get; set; }
        public string? SkillName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
