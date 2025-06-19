using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FPTAlumniConnect.BusinessTier.Payload.Education
{
    public class EducationFilter
    {
        public string? SchoolName { get; set; }
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }


    }
}