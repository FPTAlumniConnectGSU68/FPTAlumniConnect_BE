using FPTAlumniConnect.DataTier.Enums;

namespace FPTAlumniConnect.DataTier.Models;
public partial class Cv
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string Gender { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Location { get; set; }

    public string Language { get; set; } = null!;

    public string LanguageLevel { get; set; } = null!;

    public int MinSalary { get; set; }

    public int MaxSalary { get; set; }

    public bool? IsDeal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    // Các trường mới
    public string? DesiredJob { get; set; }

    public string? Position { get; set; }

    public int? MajorId { get; set; }

    public string? AdditionalContent { get; set; }

    public CVStatus Status { get; set; }

    // New fields for school/university history
    public string? SchoolName { get; set; } // Name of the school or university
    public string? Degree { get; set; } // Degree earned (e.g., Bachelor, Master)
    public string? FieldOfStudy { get; set; } // Field of study or major
    public int? GraduationYear { get; set; } // Year of graduation
    public string? EducationDescription { get; set; } // Additional details about education

    // Quan hệ
    public virtual User? User { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    public virtual ICollection<TagJob> TagJobs { get; set; } = new List<TagJob>();

    public virtual ICollection<CvSkill> CvSkills { get; set; } = new List<CvSkill>();

    public virtual MajorCode? Major { get; set; }  // FK tới MajorCode
    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}
