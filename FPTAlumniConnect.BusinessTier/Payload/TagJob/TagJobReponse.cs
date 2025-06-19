using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.TagJob
{
    public class TagJobReponse
    {
        public int TagJobId { get; set; }

        public string? Tag { get; set; }

        public int? CvID { get; set; }
    }
}
