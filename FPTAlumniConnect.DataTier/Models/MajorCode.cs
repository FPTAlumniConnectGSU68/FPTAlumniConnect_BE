﻿namespace FPTAlumniConnect.DataTier.Models;

public partial class MajorCode
{
    public int MajorId { get; set; }    // PK

    public string MajorName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<SpMajorCode> SpMajorCodes { get; set; } = new List<SpMajorCode>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
