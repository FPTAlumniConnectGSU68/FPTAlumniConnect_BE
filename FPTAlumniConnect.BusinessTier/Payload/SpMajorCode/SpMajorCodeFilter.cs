using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.SpMajorCode
{
    public class SpMajorCodeFilter
    {
        public int? MajorId { get; set; }

        public string? MajorName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
