using FPTAlumniConnect.BusinessTier.Payload.Schedule;

namespace FPTAlumniConnect.BusinessTier.Payload.Mentorship;

public class MentorshipReponse
{
    public int Id { get; set; }

    public int? AumniId { get; set; }
    public string? AlumniName { get; set; }

    public string? RequestMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public List<ScheduleResponse>? Schedules { get; set; }
}
public class ScheduleResponse
{
    public int ScheduleId { get; set; }

    public int? MentorId { get; set; }

    public string? MentorName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Content { get; set; }

    public string Status { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
