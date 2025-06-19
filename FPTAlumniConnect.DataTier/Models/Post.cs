using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int? AuthorId { get; set; }

    public string Content { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? Views { get; set; }

    public int? MajorId { get; set; }

    public string Status { get; set; } = null!;

    public bool? IsPrivate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual MajorCode? Major { get; set; }

    public virtual ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();
}
