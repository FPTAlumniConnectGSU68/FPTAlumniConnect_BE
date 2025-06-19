using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? MentorShipId { get; set; }

    public int? MentorId { get; set; }

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

    public virtual User? Mentor { get; set; }

    public virtual Mentorship? MentorShip { get; set; }
}
