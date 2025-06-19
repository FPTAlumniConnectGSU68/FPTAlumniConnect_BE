using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.SkillJob
{
    public class SkillJobReponse
    {
        public int SkillJobId { get; set; }

        public string? Skill { get; set; }

        public int? CvID { get; set; }
    }
}
