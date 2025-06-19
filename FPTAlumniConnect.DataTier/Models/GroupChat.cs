using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;

public partial class GroupChat
{
    public int Id { get; set; }

    public string RoomName { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<GroupChatMember> GroupChatMembers { get; set; } = new List<GroupChatMember>();
}
