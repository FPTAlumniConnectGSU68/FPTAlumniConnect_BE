using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.WorkExperience
{
    public class WorkExperienceInfo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CompanyWebsite { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public int UserId { get; set; }
    }

}