namespace FPTAlumniConnect.BusinessTier.Payload.PrivacySetting
{
    public class PrivacySettingFilter
    {
        public int? UserId { get; set; }
        public bool? VisibleToEducationHistory { get; set; }
        public bool? VisibleToMajor { get; set; }
        public bool? VisibleToEmail { get; set; }
        public bool? VisibleToAlumni { get; set; }
    }
}