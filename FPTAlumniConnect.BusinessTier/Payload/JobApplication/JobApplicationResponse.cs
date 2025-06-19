using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.JobApplication
{
    public class JobApplicationResponse
    {
        public int ApplicationId { get; set; }

        public int? JobPostId { get; set; }

        public int? Cvid { get; set; }

        public string? LetterCover { get; set; }

        public string Status { get; set; } = null!;

        public string? Type { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
