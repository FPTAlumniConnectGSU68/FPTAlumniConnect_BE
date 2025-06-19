using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Comment;

public class CommentFilter
{
    public int? PostId { get; set; }

    public int? AuthorId { get; set; }

    public int? ParentCommentId { get; set; }

    public string? Type { get; set; }
    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

}
