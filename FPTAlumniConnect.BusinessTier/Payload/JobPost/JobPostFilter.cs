namespace FPTAlumniConnect.BusinessTier.Payload.JobPost
{
    public class JobPostFilter
    {
        public int? UserId { get; set; }

        public int? MajorId { get; set; }

        public int? MinSalary { get; set; }

        public int? MaxSalary { get; set; }

        public bool? IsDeal { get; set; }

        public string? Location { get; set; }

        public string? City { get; set; }

        public string? Status { get; set; }

        public DateTime? Time { get; set; }

        //public DateTime? CreatedAt { get; set; }

        //public DateTime? UpdatedAt { get; set; }
    }

}
