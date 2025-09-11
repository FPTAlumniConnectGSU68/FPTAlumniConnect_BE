using System.ComponentModel.DataAnnotations;

namespace FPTAlumniConnect.DataTier.Models;

public partial class EmploymentHistory
{
    [Key]
    public int EmploymentHistoryId { get; set; } // Primary key for EmploymentHistory

    public int CvId { get; set; } // Foreign key to Cv

    public string CompanyName { get; set; } = null!; // Name of the company

    public string PrimaryDuties { get; set; } = null!; // Main responsibilities in the job

    public string JobLevel { get; set; } = null!; // Job level (e.g., Junior, Senior)

    public DateTime? StartDate { get; set; } // Start date of the job

    public DateTime? EndDate { get; set; } // End date of the job, nullable for ongoing jobs

    public bool IsCurrentJob { get; set; } // Indicates if the job is ongoing (true if still working)

    // Navigation property
    public virtual Cv Cv { get; set; } = null!; // Relationship to Cv
}