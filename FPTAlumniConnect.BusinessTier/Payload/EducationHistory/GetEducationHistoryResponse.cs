using System;

namespace FPTAlumniConnect.BusinessTier.Payload.EducationHistory
{
    public class GetEducationHistoryResponse
    {
        public int EduHistoryId { get; set; }

        public int? Iduser { get; set; }

        public string Name { get; set; } = null!;

        public DateTime ReceivedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }
}