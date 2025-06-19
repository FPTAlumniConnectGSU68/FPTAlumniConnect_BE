using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class JobApplication
{
    public int ApplicationId { get; set; }

    public int? JobPostId { get; set; }

    public int? Cvid { get; set; }

    public string? LetterCover { get; set; }

    public string Status { get; set; } = null!;

    public string? Type { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Cv? Cv { get; set; }

    public virtual JobPost? JobPost { get; set; }
}
