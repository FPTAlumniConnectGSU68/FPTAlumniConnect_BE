namespace FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo
{
    public class RecruiterInfoInfo
    {
        public int UserId { get; set; }

        public string? Status { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyEmail { get; set; }

        public string? CompanyPhone { get; set; }

        public string? CompanyLogoUrl { get; set; }

        public string? CompanyCertificateUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

}
