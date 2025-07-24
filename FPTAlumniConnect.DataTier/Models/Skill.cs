using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models
{
    public partial class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CvSkill> CvSkills { get; set; } = new List<CvSkill>();
        public virtual ICollection<JobPostSkill> JobPostSkills { get; set; } = new List<JobPostSkill>();

    }
}
