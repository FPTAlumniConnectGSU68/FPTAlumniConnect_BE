using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.PostReport
{
    public class PostReportInfo
    {
        public string? TypeOfReport { get; set; }

        public int? PostId { get; set; }

        public int? UserId { get; set; }
    }
}
