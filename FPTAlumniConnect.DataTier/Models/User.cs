using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.DataTier.Models;
public partial class User
{
    public int UserId { get; set; }
    public string? Code { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? EmailVerified { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public int RoleId { get; set; }

    public int? MajorId { get; set; }

    public string? GoogleId { get; set; }

    public bool? IsMentor { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<EducationHistory> EducationHistories { get; set; } = new List<EducationHistory>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<GroupChatMember> GroupChatMembers { get; set; } = new List<GroupChatMember>();

    public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    public virtual MajorCode? Major { get; set; }

    public virtual ICollection<Mentorship> Mentorships { get; set; } = new List<Mentorship>();

    public virtual ICollection<NotificationSetting> NotificationSettings { get; set; } = new List<NotificationSetting>();

    public virtual ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<PrivacySetting> PrivacySettings { get; set; } = new List<PrivacySetting>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<SoicalLink> SoicalLinks { get; set; } = new List<SoicalLink>();

    public virtual ICollection<UserJoinEvent> UserJoinEvents { get; set; } = new List<UserJoinEvent>();
    // New relationships for WorkExperience and Education
    public virtual ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
    public virtual ICollection<Education> EducationRecords { get; set; } = new List<Education>();
}