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

            entity.HasIndex(e => e.schedule_id, "FK_Attendance_Schedule");

            entity.HasIndex(e => e.user_id, "FK_Attendance_User");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.schedule).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.schedule_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Schedule");

            entity.HasOne(d => d.user).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_User");
        });

        modelBuilder.Entity<CV_Info>(entity =>
        {
            entity.HasKey(e => e.cvInfo_id).HasName("PRIMARY");

            entity.ToTable("CV_Info");

            entity.HasIndex(e => e.recruitment_id, "FK_CVInfo_Recruitment");

            entity.HasIndex(e => e.file_id, "FK_CVInfo_UserFile");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.education).HasMaxLength(255);
            entity.Property(e => e.gpa).HasPrecision(2, 1);
            entity.Property(e => e.skill).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.file).WithMany(p => p.CV_Infos)
                .HasForeignKey(d => d.file_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CVInfo_UserFile");

            entity.HasOne(d => d.recruitment).WithMany(p => p.CV_Infos)
                .HasForeignKey(d => d.recruitment_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CVInfo_Recruitment");
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

            entity.HasIndex(e => e.mentor_id, "FK_Class_Mentor");

            entity.Property(e => e.class_description).HasMaxLength(255);
            entity.Property(e => e.class_name).HasMaxLength(255);
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.mentor).WithMany(p => p.Classes)
                .HasForeignKey(d => d.mentor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Class_Mentor");
        });

        modelBuilder.Entity<Completed_Task>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.class_id, "FK_CompletedTasks_Class");

            entity.HasIndex(e => e.task_id, "FK_CompletedTasks_Task");

            entity.HasIndex(e => e.user_id, "FK_CompletedTasks_User");

            entity.Property(e => e.comment).HasColumnType("text");
            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.file).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d._class).WithMany()
                .HasForeignKey(d => d.class_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedTasks_Class");

            entity.HasOne(d => d.task).WithMany()
                .HasForeignKey(d => d.task_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedTasks_Task");

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedTasks_User");
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

            entity.HasIndex(e => e.conversation_id, "FK_ConvUser_Conv");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.conversation).WithMany(p => p.Conversation_users)
                .HasForeignKey(d => d.conversation_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConvUser_Conv");

            entity.HasOne(d => d.user).WithMany(p => p.Conversation_users)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConvUser_User");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("PRIMARY");

            entity.ToTable("message");

            entity.HasIndex(e => e.conversation_id, "FK_Message_Conversation");

            entity.HasIndex(e => e.created_by, "FK_Message_Sender");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.message_content).HasColumnType("text");
            entity.Property(e => e.message_type).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.conversation_id)
                .HasConstraintName("FK_Message_Conversation");

            entity.HasOne(d => d.sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("FK_Message_Sender");
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

            entity.HasIndex(e => e.notification_id, "FK_NotiRec_Notification");

            entity.HasIndex(e => e.recipient_id, "FK_NotiRec_Recipient");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.notification).WithMany(p => p.Notification_recipients)
                .HasForeignKey(d => d.notification_id)
                .HasConstraintName("FK_NotiRec_Notification");

            entity.HasOne(d => d.recipient).WithMany(p => p.Notification_recipients)
                .HasForeignKey(d => d.recipient_id)
                .HasConstraintName("FK_NotiRec_Recipient");
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Recruitment");

            entity.HasIndex(e => e.class_id, "FK_Recruitment_Class");

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

            entity.HasOne(d => d._class).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.class_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruitment_Class");
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

            entity.HasIndex(e => e.class_id, "FK_Schedule_Class");

            entity.HasIndex(e => e.mentor_id, "FK_Schedule_Mentor");

            entity.HasIndex(e => e.room_id, "FK_Schedule_Room");

            entity.HasIndex(e => e.subject_id, "FK_Schedule_Subject");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.day_of_week).HasMaxLength(10);
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.end_time).HasColumnType("time");
            entity.Property(e => e.start_time).HasColumnType("time");
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d._class).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.class_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Class");

            entity.HasOne(d => d.mentor).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.mentor_id)
                .HasConstraintName("FK_Schedule_Mentor");

            entity.HasOne(d => d.room).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.room_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Room");

            entity.HasOne(d => d.subject).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.subject_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Subject");
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

            entity.HasIndex(e => e.submitter_id, "FK_UserFile_Submitter");

            entity.HasIndex(e => new { e.id, e.submitter_id }, "uq_submitter_file").IsUnique();

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.display_name).HasMaxLength(255);
            entity.Property(e => e.path).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d.submitter).WithMany(p => p.UserFiles)
                .HasForeignKey(d => d.submitter_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFile_Submitter");
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.class_id, "FK_UserTasks_Class");

            entity.HasIndex(e => e.created_by, "FK_UserTasks_Creator");

            entity.Property(e => e.created_at).HasColumnType("datetime");
            entity.Property(e => e.deleted_at).HasColumnType("datetime");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.end_time).HasColumnType("datetime");
            entity.Property(e => e.file).HasMaxLength(255);
            entity.Property(e => e.start_time).HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.task_name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("datetime");

            entity.HasOne(d => d._class).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.class_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTasks_Class");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTasks_Creator");
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