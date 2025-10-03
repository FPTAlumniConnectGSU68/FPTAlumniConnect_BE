using System.ComponentModel.DataAnnotations;

namespace FPTAlumniConnect.DataTier.Models;

public partial class EventTimeLine
{
    [Key]
    public int EventTimeLineId { get; set; }

    public int EventId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Speaker { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public DateTime? Day { get; set; } // Added field for the day of StartTime and EndTime

    public virtual Event Event { get; set; }
}