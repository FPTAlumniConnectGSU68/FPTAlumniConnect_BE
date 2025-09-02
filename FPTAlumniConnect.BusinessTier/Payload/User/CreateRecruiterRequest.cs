using System.ComponentModel.DataAnnotations;

namespace FPTAlumniConnect.BusinessTier.Payload
{
    // Request payload for creating a recruiter, combining user and recruiter info
    public class CreateRecruiterRequest
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid company email format")]
        public string? CompanyEmail { get; set; }

        [Phone(ErrorMessage = "Invalid company phone format")]
        public string? CompanyPhone { get; set; }

        public string? CompanyLogoUrl { get; set; }

        public string? CompanyCertificateUrl { get; set; }

        [RegularExpression("^(Pending|Active|Inactive)$", ErrorMessage = "Status must be 'Pending', 'Active', or 'Inactive'")]
        public string Status { get; set; } = "Pending";
    }

    // Response payload for creating a recruiter
    public class CreateRecruiterResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RecruiterInfoId { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}