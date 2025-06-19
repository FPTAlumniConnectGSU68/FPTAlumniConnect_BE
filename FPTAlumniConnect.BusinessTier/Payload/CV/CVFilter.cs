using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVFilter
    {
        public int? UserId { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? Email { get; set; }

        public string? City { get; set; }

        public string? Company { get; set; }

        public string? PrimaryDuties { get; set; } 

        public string? JobLevel { get; set; } 

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public string? Language { get; set; }

        public string? LanguageLevel { get; set; }

        public string? MinSalary { get; set; } 

        public string? MaxSalary { get; set; } 

        public bool? IsDeal { get; set; }
    }
}
