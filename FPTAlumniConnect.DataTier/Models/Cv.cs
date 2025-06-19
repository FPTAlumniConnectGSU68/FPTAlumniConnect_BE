using System;
using System.Collections.Generic;

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

    public string Company { get; set; } = null!;

    public string PrimaryDuties { get; set; } = null!;

    public string JobLevel { get; set; } = null!;

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public string Language { get; set; } = null!;

    public string LanguageLevel { get; set; } = null!;

    public int MinSalary { get; set; }

    public int MaxSalary { get; set; }

    public bool? IsDeal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    public virtual ICollection<TagJob> TagJobs { get; set; } = new List<TagJob>();

    public virtual ICollection<SkillJob> SkillJobs { get; set; } = new List<SkillJob>();

    public virtual User? User { get; set; }
}
