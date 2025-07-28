namespace FPTAlumniConnect.DataTier.Models
{
    public partial class JobPostSkill
    {
        public int JobPostId { get; set; }
        public int SkillId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual JobPost JobPost { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}
