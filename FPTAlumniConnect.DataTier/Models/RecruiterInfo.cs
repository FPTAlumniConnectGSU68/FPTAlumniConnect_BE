using System.ComponentModel.DataAnnotations;

namespace FPTAlumniConnect.DataTier.Models
{
    public class RecruiterInfo
    {
        [Key]
        public int RecruiterInfoId { get; set; }

        public int UserId { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyEmail { get; set; }

        public string? CompanyPhone { get; set; }

        public string? CompanyLogoUrl { get; set; }

        public string? CompanyCertificateUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }

}
