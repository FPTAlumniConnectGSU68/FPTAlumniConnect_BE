namespace FPTAlumniConnect.DataTier.Models
{
    public partial class CvSkill
    {
        public int CvId { get; set; }
        public int SkillId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Cv Cv { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}
