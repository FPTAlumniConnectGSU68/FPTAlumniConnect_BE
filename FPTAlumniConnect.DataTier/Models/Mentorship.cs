using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class Mentorship
{
    public int Id { get; set; }

    public int? AumniId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? RequestMessage { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? Aumni { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
