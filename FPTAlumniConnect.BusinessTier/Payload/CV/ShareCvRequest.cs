using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class ShareCvRequest
    {
        public int CvId { get; set; }
        public string RecipientEmail { get; set; } = null!;
        public string? Message { get; set; }
    }
}
