﻿namespace FPTAlumniConnect.BusinessTier.Payload.Schedule;

public class ScheduleReponse
{
    public int ScheduleId { get; set; }

    public int? MentorShipId { get; set; }

    public int? MentorId { get; set; }
    public int? AlumniId { get; set; }
    public string? MentorName { get; set; }
    public string? AlumniName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Content { get; set; }

    public string Status { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Comment { get; set; }
}
