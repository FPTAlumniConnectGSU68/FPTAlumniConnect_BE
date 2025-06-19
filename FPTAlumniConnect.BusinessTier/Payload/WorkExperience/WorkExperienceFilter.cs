using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.WorkExperience
{
    public class WorkExperienceFilter
    {
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public string? Location { get; set; }
        public int? UserId { get; set; }
    }
}