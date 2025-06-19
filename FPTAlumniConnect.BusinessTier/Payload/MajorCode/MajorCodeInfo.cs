using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.MajorCode
{
    public class MajorCodeInfo
    {
        public string MajorName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
