using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.PostReport
{
    public class PostReportReponse
    {
        public int RpId { get; set; }

        public string? TypeOfReport { get; set; }

        public int? PostId { get; set; }

        public int? UserId { get; set; }
    }
}
