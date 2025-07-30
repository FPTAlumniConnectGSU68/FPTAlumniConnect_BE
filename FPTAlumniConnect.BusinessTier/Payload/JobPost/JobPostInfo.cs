using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.JobPost
{
    public class JobPostInfo
    {
        public string? JobTitle { get; set; }

        public string? JobDescription { get; set; }

        public int? MinSalary { get; set; }

        public int? MaxSalary { get; set; }

        public bool? IsDeal { get; set; }

        public string? Location { get; set; }

        public string? City { get; set; }

        public string? Requirements { get; set; }

        public string? Benefits { get; set; }

        public DateTime? Time { get; set; }

        public string? Status { get; set; }

        public string? Email { get; set; }

        public int? UserId { get; set; }

        public int? MajorId { get; set; }
    }

}
