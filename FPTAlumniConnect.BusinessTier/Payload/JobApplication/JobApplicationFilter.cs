using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.JobApplication
{
    public class JobApplicationFilter
    {
        public int? JobPostId { get; set; }

        public int? Cvid { get; set; }

        public string? Status { get; set; }

        public string? Type { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
