using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.JobApplication
{
    public class JobApplicationInfo
    {
        public int? JobPostId { get; set; }

        public int? Cvid { get; set; }

        public string? LetterCover { get; set; }

        public string Status { get; set; } = null!;

        public string? Type { get; set; }
    }
}
