using System;

namespace FPTAlumniConnect.BusinessTier.Payload.EducationHistory
{
    public class EducationHistoryFilter
    {
        public int? Iduser { get; set; }

        public string? Name { get; set; }

        public DateTime? ReceivedAtFrom { get; set; }

        public DateTime? ReceivedAtTo { get; set; }

        public DateTime? CreatedAtFrom { get; set; }

        public DateTime? CreatedAtTo { get; set; }

        public string? CreatedBy { get; set; }
    }
}