using System;

namespace FPTAlumniConnect.BusinessTier.Payload.SocialLink
{
    public class GetSocialLinkResponse
    {
        public int Slid { get; set; }
        public int? UserId { get; set; }
        public string Link { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}