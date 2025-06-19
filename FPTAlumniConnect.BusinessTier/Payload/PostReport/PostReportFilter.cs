using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.PostReport
{
    public class PostReportFilter
    {
        public int? PostId { get; set; }

        public int? UserId { get; set; }

        public string? TypeOfReport { get; set; }
    }
}
