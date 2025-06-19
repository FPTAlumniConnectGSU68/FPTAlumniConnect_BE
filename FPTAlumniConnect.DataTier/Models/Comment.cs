using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? PostId { get; set; }

    public int? AuthorId { get; set; }

    public string Content { get; set; } = null!;
    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ParentCommentId { get; set; }

    public string? Type { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? Author { get; set; }

    public virtual Post? Post { get; set; }
}
