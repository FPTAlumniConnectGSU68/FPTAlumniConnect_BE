using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Schedule;

public class ScheduleFilter
{
    public int? MentorShipId { get; set; }

    public int? MentorId { get; set; }

    public string? Status { get; set; } = null!;
}
