using FPTAlumniConnect.DataTier.Models;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        public string? Company { get; set; }

        public string? PrimaryDuties { get; set; }

        public string? JobLevel { get; set; }

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

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

        // New field to handle multiple CV skills
        public List<int>? SkillIds { get; set; } // List of Skill IDs to associate with the CV
    }
}