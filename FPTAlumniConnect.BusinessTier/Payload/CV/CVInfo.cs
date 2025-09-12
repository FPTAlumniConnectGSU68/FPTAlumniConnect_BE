using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Models;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVInfo
    {
        public int UserId { get; set; }

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Gender { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? City { get; set; }

        public List<EmploymentHistoryInfo>? EmploymentHistories { get; set; } // List of employment history entries

        public string? Language { get; set; }

        public string? LanguageLevel { get; set; }

        public int? MinSalary { get; set; }

        public int? MaxSalary { get; set; }

        public bool? IsDeal { get; set; }

        public string? DesiredJob { get; set; }

        public string? Position { get; set; }

        public int? MajorId { get; set; }

        public string? AdditionalContent { get; set; }

        public string? Status { get; set; }

        public List<int>? SkillIds { get; set; } // List of Skill IDs to associate with the CV
    }
}