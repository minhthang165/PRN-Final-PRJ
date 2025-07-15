using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Entities;

namespace PRN_Final_Project.Business.Data;

public partial class PRNDbContext : DbContext
{
    public PRNDbContext(DbContextOptions<PRNDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<CV_Info> CV_Infos { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Completed_Task> Completed_Tasks { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Conversation_user> Conversation_users { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Notification_recipient> Notification_recipients { get; set; }

    public virtual DbSet<Recruitment> Recruitments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<UserFile> UserFiles { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.attendance_id).HasName("PRIMARY");

            entity.ToTable("Attendance");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<CV_Info>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CV_Info");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.education).HasMaxLength(255);
            entity.Property(e => e.gpa).HasPrecision(2, 1);
            entity.Property(e => e.skill).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(200)
                .IsFixedLength();
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Class");

            entity.Property(e => e.class_description).HasMaxLength(255);
            entity.Property(e => e.class_name).HasMaxLength(255);
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Completed_Task>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.comment).HasColumnType("text");
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.file).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.conversation_id).HasName("PRIMARY");

            entity.ToTable("Conversation");

            entity.Property(e => e.conversation_avatar).HasMaxLength(255);
            entity.Property(e => e.conversation_name).HasMaxLength(255);
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.type).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Conversation_user>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.conversation_id })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("Conversation_user");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("PRIMARY");

            entity.ToTable("Message");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.message_content).HasColumnType("text");
            entity.Property(e => e.message_type).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Notification");

            entity.Property(e => e.content).HasColumnType("text");
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.notification_type).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
            entity.Property(e => e.url).HasColumnType("text");
        });

        modelBuilder.Entity<Notification_recipient>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Notification_recipient");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Recruitment");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.end_time).HasColumnType("datetime");
            entity.Property(e => e.experience_requirement).HasMaxLength(255);
            entity.Property(e => e.language).HasMaxLength(255);
            entity.Property(e => e.min_GPA).HasPrecision(2, 1);
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.position).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Room");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.location).HasMaxLength(255);
            entity.Property(e => e.room_name).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Schedule");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.day_of_week).HasMaxLength(10);
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.end_time).HasColumnType("time");
            entity.Property(e => e.start_time).HasColumnType("time");
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Subject");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.subject_name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserFile>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("UserFile");

            entity.HasIndex(e => new { e.id, e.submitter_id }, "uq_submitter_file").IsUnique();

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.display_name).HasMaxLength(255);
            entity.Property(e => e.path).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.end_time).HasColumnType("datetime");
            entity.Property(e => e.file).HasMaxLength(255);
            entity.Property(e => e.start_time).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.task_name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.avatar_path).HasColumnType("text");
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.first_name).HasMaxLength(255);
            entity.Property(e => e.gender).HasMaxLength(50);
            entity.Property(e => e.last_name).HasMaxLength(255);
            entity.Property(e => e.phone_number).HasMaxLength(255);
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");
            entity.Property(e => e.user_name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
