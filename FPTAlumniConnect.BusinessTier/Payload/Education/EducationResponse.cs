using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FPTAlumniConnect.BusinessTier.Payload.Education
{
    public class EducationResponse
    {
        public int Id { get; set; }
        public string SchoolName { get; set; } = null!;
        public string Major { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SchoolWebsite { get; set; } = null!;
        public string Achievements { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
    }

}