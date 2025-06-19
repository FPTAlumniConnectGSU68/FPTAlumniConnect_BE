using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Comment;

public class CommentReponse
{
    public int CommentId { get; set; }

    public int? PostId { get; set; }

    public int? AuthorId { get; set; }

    public string Content { get; set; } = null!;

    public int? ParentCommentId { get; set; }

    public string? Type { get; set; }
    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
