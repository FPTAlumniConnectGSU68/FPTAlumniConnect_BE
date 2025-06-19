using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class GroupChatMember
{
    public int Id { get; set; }

    public int? GroupChatId { get; set; }

    public int? UserId { get; set; }

    public bool? IsOwner { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual GroupChat? GroupChat { get; set; }

    public virtual ICollection<MessageGroupChat> MessageGroupChats { get; set; } = new List<MessageGroupChat>();

    public virtual User? User { get; set; }
}
