using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class PostReport
{
    public int RpId { get; set; }

    public int? PostId { get; set; }

    public int? UserId { get; set; }

    public string? TypeOfReport { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
