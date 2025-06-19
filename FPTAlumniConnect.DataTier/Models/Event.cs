using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Img { get; set; }

    public string? Description { get; set; }
    public bool? Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Location { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
    public int? OrganizerId { get; set; }
    public virtual User? Organizer { get; set; }
    public virtual ICollection<UserJoinEvent> UserJoinEvents { get; set; } = new List<UserJoinEvent>();
    public virtual ICollection<EventTimeLine> EventTimeLines { get; set; } = new List<EventTimeLine>();
}
