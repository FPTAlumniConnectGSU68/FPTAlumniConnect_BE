using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.MajorCode
{
    public class MajorCodeFilter
    {
        public string? MajorName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
