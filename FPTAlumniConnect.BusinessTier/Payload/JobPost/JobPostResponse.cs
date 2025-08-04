using FPTAlumniConnect.BusinessTier.Payload.SkillJob;

public class JobPostResponse
{
    public int JobPostId { get; set; }
    public string JobDescription { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string? Location { get; set; }
    public string? City { get; set; }
    public int? MinSalary { get; set; }
    public int? MaxSalary { get; set; }
    public bool IsDeal { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public DateTime Time { get; set; }
    public string Status { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? UserId { get; set; }
    public int? MajorId { get; set; }
    public string? MajorName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public List<SkillResponse>? Skills { get; set; }
}

public class SkillResponse
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = null!;
}

