namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVResponse
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

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

        public int MinSalary { get; set; }

        public int MaxSalary { get; set; }

        public bool? IsDeal { get; set; }

        public string? DesiredJob { get; set; }

        public string? Position { get; set; }

        /// <summary>
        /// Foreign key to MajorCode
        /// </summary>
        public int? MajorId { get; set; }

        /// <summary>
        /// Name of the major, from MajorCode table
        /// </summary>
        public string? MajorName { get; set; }

        public string? AdditionalContent { get; set; }

        /// <summary>
        /// Status of the CV (e.g. Draft, Published)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// List of associated skill IDs
        /// </summary>
        public List<int>? SkillIds { get; set; }

        /// <summary>
        /// List of associated skill names
        /// </summary>
        public List<string>? SkillNames { get; set; }
    }
}
