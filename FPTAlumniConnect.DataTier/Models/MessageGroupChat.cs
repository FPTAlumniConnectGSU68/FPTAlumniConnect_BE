using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class MessageGroupChat
{
    public int Id { get; set; }

    public int? MemberId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual GroupChatMember? Member { get; set; }
}
