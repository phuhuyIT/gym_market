using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Models;

public partial class GymMarketContext : DbContext
{
    public GymMarketContext()
    {
    }

    public GymMarketContext(DbContextOptions<GymMarketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseOption> CourseOptions { get; set; }

    public virtual DbSet<CourseRating> CourseRatings { get; set; }

    public virtual DbSet<CourseRegistration> CourseRegistrations { get; set; }

    public virtual DbSet<CourseRegistrationOption> CourseRegistrationOptions { get; set; }

    public virtual DbSet<HealthDatum> HealthData { get; set; }

    public virtual DbSet<HealthIndicator> HealthIndicators { get; set; }

    public virtual DbSet<Lecture> Lectures { get; set; }

    public virtual DbSet<LectureMaterial> LectureMaterials { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-LMTSK20\\SQLEXPRESS;Database=gym_market;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__37E005FB71A00E15");

            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.AdditionalPrice)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Additional_Price");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.MaxParticipants).HasColumnName("Max_Participants");
            entity.Property(e => e.Price)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Rating)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Trainer).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_Courses_Trainer");
        });

        modelBuilder.Entity<CourseOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Course_O__3260905ED3772281");

            entity.ToTable("Course_Options");

            entity.Property(e => e.OptionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Option_ID");
            entity.Property(e => e.OptionName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Option_Name");
            entity.Property(e => e.Price)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<CourseRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Course_R__BE48C825152098C5");

            entity.ToTable("Course_Ratings");

            entity.Property(e => e.RatingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Rating_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.RatingValue)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("Rating_Value");
            entity.Property(e => e.ReviewComment).HasColumnName("Review_Comment");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseRatings)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Course_Ratings_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRatings)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Course_Ratings_Student");
        });

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Course_R__80BC7A177D892242");

            entity.ToTable("Course_Registration");

            entity.Property(e => e.RegistrationId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Registration_ID");
            entity.Property(e => e.AdditionalFeaturesPayment)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Additional_Features_Payment");
            entity.Property(e => e.ContractAgreement)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Contract_Agreement");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.InitialPayment)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Initial_Payment");
            entity.Property(e => e.Mode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Status");
            entity.Property(e => e.RegistrationType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Registration_Type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Course_Registration_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Course_Registration_Student");
        });

        modelBuilder.Entity<CourseRegistrationOption>(entity =>
        {
            entity.HasKey(e => e.RegistrationOptionId).HasName("PK__Course_R__69107FA1C5F3CCD3");

            entity.ToTable("Course_Registration_Options");

            entity.Property(e => e.RegistrationOptionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Registration_Option_ID");
            entity.Property(e => e.OptionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Option_ID");
            entity.Property(e => e.RegistrationId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Registration_ID");

            entity.HasOne(d => d.Option).WithMany(p => p.CourseRegistrationOptions)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK_Course_Registration_Options_Option");

            entity.HasOne(d => d.Registration).WithMany(p => p.CourseRegistrationOptions)
                .HasForeignKey(d => d.RegistrationId)
                .HasConstraintName("FK_Course_Registration_Options_Registration");
        });

        modelBuilder.Entity<HealthDatum>(entity =>
        {
            entity.HasKey(e => e.HealthDataId).HasName("PK__Health_D__AC2A295B64A6A474");

            entity.ToTable("Health_Data");

            entity.Property(e => e.HealthDataId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Health_Data_ID");
            entity.Property(e => e.BodyImage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Body_Image");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.ProgressNotes).HasColumnName("Progress_Notes");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasOne(d => d.Course).WithMany(p => p.HealthData)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Health_Data_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.HealthData)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Health_Data_Student");
        });

        modelBuilder.Entity<HealthIndicator>(entity =>
        {
            entity.HasKey(e => e.IndicatorId).HasName("PK__Health_I__7798E6A1AA3F762E");

            entity.ToTable("Health_Indicators");

            entity.Property(e => e.IndicatorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Indicator_ID");
            entity.Property(e => e.HealthDataId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Health_Data_ID");
            entity.Property(e => e.IndicatorName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Indicator_Name");
            entity.Property(e => e.IndicatorValue).HasColumnName("Indicator_Value");
            entity.Property(e => e.MeasurementDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Measurement_Date");

            entity.HasOne(d => d.HealthData).WithMany(p => p.HealthIndicators)
                .HasForeignKey(d => d.HealthDataId)
                .HasConstraintName("FK_Health_Indicators_HealthData");
        });

        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.HasKey(e => e.LectureId).HasName("PK__Lectures__551CCF797A21E155");

            entity.Property(e => e.LectureId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lecture_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Lectures_Course");
        });

        modelBuilder.Entity<LectureMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Lecture___3A09B0FD4BA27D50");

            entity.ToTable("Lecture_Materials");

            entity.Property(e => e.MaterialId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Material_ID");
            entity.Property(e => e.LectureId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lecture_ID");
            entity.Property(e => e.MaterialContent).HasColumnName("Material_Content");
            entity.Property(e => e.MaterialType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Material_Type");

            entity.HasOne(d => d.Lecture).WithMany(p => p.LectureMaterials)
                .HasForeignKey(d => d.LectureId)
                .HasConstraintName("FK_Lecture_Materials_Lecture");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__F5A446E2FCEFE4A9");

            entity.Property(e => e.MessageId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Message_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.MessageContent).HasColumnName("Message_Content");
            entity.Property(e => e.MessageType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Message_Type");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");

            entity.HasOne(d => d.Course).WithMany(p => p.Messages)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Messages_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Messages)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Messages_Student");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Messages)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_Messages_Trainer");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__8C1160B56820950B");

            entity.Property(e => e.NotificationId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Notification_ID");
            entity.Property(e => e.NotificationContent).HasColumnName("Notification_Content");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Notification_Type");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Notifications_Student");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__DA6C7FE18EA85F75");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Payment_Amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Payment_Date");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Status");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Type");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasOne(d => d.Course).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Payments_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Payments_Student");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__A2F4E9AC7CF53A74");

            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HealthStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Health_Status");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Profile_Picture");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Students_AspNetUsers");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PK__Trainers__8B0EB931432C150B");

            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");
            entity.Property(e => e.Certification)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Profile_Picture");
            entity.Property(e => e.Rating)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Trainers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Trainers_AspNetUsers");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_UserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
