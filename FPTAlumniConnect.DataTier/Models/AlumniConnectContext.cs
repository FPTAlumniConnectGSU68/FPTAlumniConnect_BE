using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.DataTier.Models;

public partial class AlumniConnectContext : DbContext
{
    public AlumniConnectContext()
    {
    }

    public AlumniConnectContext(DbContextOptions<AlumniConnectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<EducationHistory> EducationHistories { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<GroupChat> GroupChats { get; set; }

    public virtual DbSet<GroupChatMember> GroupChatMembers { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<JobPost> JobPosts { get; set; }

    public virtual DbSet<MajorCode> MajorCodes { get; set; }

    public virtual DbSet<Mentorship> Mentorships { get; set; }

    public virtual DbSet<MessageGroupChat> MessageGroupChats { get; set; }

    public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostReport> PostReports { get; set; }

    public virtual DbSet<PrivacySetting> PrivacySettings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SkillJob> SkillJobs { get; set; }

    public virtual DbSet<SoicalLink> SoicalLinks { get; set; }

    public virtual DbSet<SpMajorCode> SpMajorCodes { get; set; }

    public virtual DbSet<TagJob> TagJobs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserJoinEvent> UserJoinEvents { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<EventTimeLine> EventTimeLines { get; set; }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Server=(local);Database=AlumniConnect;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA19A3DE73");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Comments__Author__208CD6FA");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Comments__PostID__1F98B2C1");
            entity.Property(e => e.Status)
    .HasDefaultValue(true);
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId);
            entity.Property(e => e.Message)
                  .IsRequired()
                  .HasMaxLength(500);
            entity.Property(e => e.Timestamp)
                  .HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsRead)
                  .HasDefaultValue(false);
        });
        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CV__3214EC07F4EB68AB");

            entity.ToTable("CV");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Birthday)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.Company).HasMaxLength(255);
            entity.Property(e => e.PrimaryDuties).HasMaxLength(255);
            entity.Property(e => e.JobLevel).HasMaxLength(255);
            entity.Property(e => e.StartAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Language).HasMaxLength(255);
            entity.Property(e => e.LanguageLevel).HasMaxLength(255);
            entity.Property(e => e.MinSalary).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaxSalary).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsDeal)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isDeal");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CV__UserID__3D2915A8");

            entity.Property(e => e.Status)
                .HasConversion<string>()  // Lưu enum dưới dạng string
                .HasMaxLength(50);

        });

        modelBuilder.Entity<EducationHistory>(entity =>
        {
            entity.HasKey(e => e.EduHistoryId).HasName("PK__Educatio__63181BC1D2CF6A79");

            entity.ToTable("EducationHistory");

            entity.Property(e => e.EduHistoryId).HasColumnName("EduHistoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ReceivedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.EducationHistories)
                .HasForeignKey(d => d.Iduser)
                .HasConstraintName("FK__Education__IDUse__4C6B5938");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C870B1F118B4");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Img).HasColumnType("TEXT");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.Status)
    .HasDefaultValue(false);

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("FK__Events__Organize__02084FDA");

        });


        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GroupCha__3214EC071C62797A");

            entity.ToTable("GroupChat");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoomName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GroupChats)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__GroupChat__Creat__6C190EBB");
        });

        modelBuilder.Entity<GroupChatMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GroupCha__3214EC2776D442E0");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.IsOwner).HasDefaultValueSql("((0))");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.GroupChat).WithMany(p => p.GroupChatMembers)
                .HasForeignKey(d => d.GroupChatId)
                .HasConstraintName("FK__GroupChat__Group__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.GroupChatMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupChat__UserI__72C60C4A");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__JobAppli__C93A4F795EC7C027");

            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Cvid).HasColumnName("CVID");
            entity.Property(e => e.JobPostId).HasColumnName("JobPostID");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Cv).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.Cvid)
                .HasConstraintName("FK__JobApplica__CVID__47A6A41B");

            entity.HasOne(d => d.JobPost).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.JobPostId)
                .HasConstraintName("FK__JobApplic__JobPo__46B27FE2");
        });

        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasKey(e => e.JobPostId).HasName("PK__JobPost__57689C5A3E8D6828");

            entity.ToTable("JobPost");

            entity.Property(e => e.JobPostId).HasColumnName("JobPostID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.JobTitle).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.MinSalary).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaxSalary).HasDefaultValueSql("((0))");
            entity.Property(e => e.Status).HasDefaultValue(false);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Major).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__JobPost__MajorID__31B762FC");

            entity.HasOne(d => d.User).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__JobPost__UserID__30C33EC3");
        });

        modelBuilder.Entity<MajorCode>(entity =>
        {
            entity.HasKey(e => e.MajorId).HasName("PK__MajorCod__D5B8BF91CFCD90DF");

            entity.ToTable("MajorCode");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.MajorName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
        });

        modelBuilder.Entity<Mentorship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mentorsh__3214EC276D8E333E");

            entity.ToTable("Mentorship");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AumniId).HasColumnName("AumniID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Aumni).WithMany(p => p.Mentorships)
                .HasForeignKey(d => d.AumniId)
                .HasConstraintName("FK__Mentorshi__Aumni__60A75C0F");
        });

        modelBuilder.Entity<MessageGroupChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageG__3214EC278ACF2846");

            entity.ToTable("MessageGroupChat");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Member).WithMany(p => p.MessageGroupChats)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__MessageGr__Membe__778AC167");
        });

        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3213E83F452FF6A1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.JobNotifications).HasDefaultValueSql("((1))");
            entity.Property(e => e.MessageNotifications).HasDefaultValueSql("((1))");
            entity.Property(e => e.ReceiveEmailNotifications).HasDefaultValueSql("((1))");
            entity.Property(e => e.ReceiveInAppNotifications).HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.NotificationSettings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__59C55456");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA1260383844026D");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.MajorId);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.IsPrivate).HasDefaultValueSql("((0))");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.Views).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Posts__AuthorID__18EBB532");

            entity.HasOne(d => d.Major).WithMany(p => p.Posts)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__Posts__MajorId__19DFD96B");
        });

        modelBuilder.Entity<PostReport>(entity =>
        {
            entity.HasKey(e => e.RpId).HasName("PK__PostRepo__DBD62899743C565C");

            entity.Property(e => e.RpId).HasColumnName("RpID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.TypeOfReport).HasMaxLength(255);
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostReports)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__PostRepor__PostI__25518C17");

            entity.HasOne(d => d.User).WithMany(p => p.PostReports)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PostRepor__UserI__2645B050");
        });

        modelBuilder.Entity<PrivacySetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PrivacyS__3213E83F8BE3191C");

            entity.ToTable("PrivacySetting");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.VisibleToAlumni).HasDefaultValueSql("((1))");
            entity.Property(e => e.VisibleToEducationHistory).HasDefaultValueSql("((1))");
            entity.Property(e => e.VisibleToEmail).HasDefaultValueSql("((1))");
            entity.Property(e => e.VisibleToMajor).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.User).WithMany(p => p.PrivacySettings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PrivacySe__UserI__625A9A57");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A0E2F5438");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69F957490C");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.MentorId).HasColumnName("MentorID");
            entity.Property(e => e.MentorShipId).HasColumnName("MentorShipID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Mentor).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.MentorId)
                .HasConstraintName("FK__Schedule__Mentor__6754599E");

            entity.HasOne(d => d.MentorShip).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.MentorShipId)
                .HasConstraintName("FK__Schedule__Mentor__66603565");
        });

        modelBuilder.Entity<SoicalLink>(entity =>
        {
            entity.HasKey(e => e.Slid).HasName("PK__SoicalLi__A43D35CFADE62797");

            entity.ToTable("SoicalLink");

            entity.Property(e => e.Slid).HasColumnName("SLID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.SoicalLinks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SoicalLin__UserI__51300E55");
        });

        modelBuilder.Entity<SpMajorCode>(entity =>
        {
            entity.HasKey(e => e.SpMajorId).HasName("PK__SpMajorC__181F8B5D69DA480D");

            entity.ToTable("SpMajorCode");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.MajorName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Major).WithMany(p => p.SpMajorCodes)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__SpMajorCo__Major__2B0A656D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC5DE5B589");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EmailVerified).HasDefaultValueSql("((0))");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsRequired(false);
            entity.Property(e => e.GoogleId).HasColumnName("GoogleID");
            entity.Property(e => e.IsMentor)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isMentor");
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProfilePicture)
             .HasColumnType("text");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Major).WithMany(p => p.Users)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__Users__MajorId__5CD6CB2B");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__5BE2A6F2");
        });

        modelBuilder.Entity<UserJoinEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserJoin__3214EC2704503508");

            entity.ToTable("UserJoinEvent");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.UserJoinEvents)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__UserJoinE__Event__0C85DE4D");

            entity.HasOne(d => d.User).WithMany(p => p.UserJoinEvents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserJoinE__UserI__0B91BA14");
        });

        modelBuilder.Entity<TagJob>(entity =>
        {
            entity.HasKey(e => e.TagJobId);

            entity.ToTable("TagJob");
            entity.Property(e => e.TagJobId);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Tag).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Cv).WithMany(p => p.TagJobs)
                .HasForeignKey(d => d.CvID);
        });

        modelBuilder.Entity<SkillJob>(entity =>
        {
            entity.HasKey(e => e.SkillJobId);

            entity.ToTable("SkillJob");
            entity.Property(e => e.SkillJobId);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Skill).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Cv).WithMany(p => p.SkillJobs)
                .HasForeignKey(d => d.CvID);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}