using System;

namespace FPTAlumniConnect.BusinessTier.Payload.PrivacySetting
{
    public class GetPrivacySettingResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public bool? VisibleToEducationHistory { get; set; }
        public bool? VisibleToMajor { get; set; }
        public bool? VisibleToEmail { get; set; }
        public bool? VisibleToAlumni { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}