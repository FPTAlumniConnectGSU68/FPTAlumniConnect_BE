namespace FPTAlumniConnect.BusinessTier.Payload.JobPost
{
    public class JobPostInfo
    {
        public string JobTitle { get; set; } = null!;

        public string JobDescription { get; set; } = null!;

        public int? MinSalary { get; set; }

        public int? MaxSalary { get; set; }

        public bool? IsDeal { get; set; }

        public string? Location { get; set; }

        public string? City { get; set; }

        public string? Requirements { get; set; }

        public string? Benefits { get; set; }

        public DateTime Time { get; set; }

        public string Status { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? UserId { get; set; }

        public int? MajorId { get; set; }
    }

}
