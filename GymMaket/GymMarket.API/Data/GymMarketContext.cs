using System;
using System.Collections.Generic;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Data;

public partial class GymMarketContext : IdentityDbContext<AppUser>
{
    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAssignment> CourseAssignments { get; set; }

    public virtual DbSet<AssignmentRubricCriterion> AssignmentRubricCriteria { get; set; }

    public virtual DbSet<AssignmentRubricScore> AssignmentRubricScores { get; set; }

    public virtual DbSet<CourseAnnouncement> CourseAnnouncements { get; set; }

    public virtual DbSet<CourseLiveSession> CourseLiveSessions { get; set; }

    public virtual DbSet<CourseDiscussionQuestion> CourseDiscussionQuestions { get; set; }

    public virtual DbSet<CourseDiscussionAnswer> CourseDiscussionAnswers { get; set; }

    public virtual DbSet<CourseStudyGroup> CourseStudyGroups { get; set; }

    public virtual DbSet<CourseOption> CourseOptions { get; set; }

    public virtual DbSet<CourseCertificate> CourseCertificates { get; set; }

    public virtual DbSet<CourseCertificateSetting> CourseCertificateSettings { get; set; }

    public virtual DbSet<CourseRating> CourseRatings { get; set; }

    public virtual DbSet<CourseRegistration> CourseRegistrations { get; set; }

    public virtual DbSet<CourseRegistrationOption> CourseRegistrationOptions { get; set; }

    public virtual DbSet<ClassBooking> ClassBookings { get; set; }

    public virtual DbSet<GymClassSession> GymClassSessions { get; set; }

    public virtual DbSet<CourseQuiz> CourseQuizzes { get; set; }

    public virtual DbSet<CourseModule> CourseModules { get; set; }

    public virtual DbSet<GradeCategory> GradeCategories { get; set; }

    public virtual DbSet<HealthDatum> HealthData { get; set; }

    public virtual DbSet<HealthIndicator> HealthIndicators { get; set; }

    public virtual DbSet<Lecture> Lectures { get; set; }

    public virtual DbSet<LectureMaterial> LectureMaterials { get; set; }

    public virtual DbSet<LectureProgress> LectureProgresses { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MembershipPlan> MembershipPlans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationDeliveryLog> NotificationDeliveryLogs { get; set; }

    public virtual DbSet<NotificationPreference> NotificationPreferences { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentEvent> PaymentEvents { get; set; }

    public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }

    public virtual DbSet<QuizAttemptAnswer> QuizAttemptAnswers { get; set; }

    public virtual DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }

    public virtual DbSet<QuizOption> QuizOptions { get; set; }

    public virtual DbSet<QuizQuestion> QuizQuestions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentMembership> StudentMemberships { get; set; }

    public virtual DbSet<StudentProgressGoal> StudentProgressGoals { get; set; }

    public virtual DbSet<StudentProgressLog> StudentProgressLogs { get; set; }

    public virtual DbSet<StudentWorkoutAssignment> StudentWorkoutAssignments { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<WorkoutExercise> WorkoutExercises { get; set; }

    public virtual DbSet<WorkoutExerciseCompletion> WorkoutExerciseCompletions { get; set; }

    public virtual DbSet<WorkoutPlan> WorkoutPlans { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }
    public virtual DbSet<UserMessage> UserMessages { get; set; }
    public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public virtual DbSet<FileCourse> FileCourses { get; set; }
    public virtual DbSet<FoodNutrition> FoodNutritions { get; set; }
    public virtual DbSet<FoodNutritionUser> FoodNutritionUsers { get; set; }
    public virtual DbSet<NutritionBudget> NutritionBudgets { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public GymMarketContext(DbContextOptions<GymMarketContext> options)
        : base(options)
    {
    }
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);

        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(e => e.FullName);
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.Status);
        });

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
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(CourseStatus.Published);
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

            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.TrainerId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(d => d.Trainer).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_Courses_Trainer");
        });

        modelBuilder.Entity<GradeCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("Grade_Categories");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Category_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(120);
            entity.Property(e => e.WeightPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Weight_Percent");
            entity.Property(e => e.Order)
                .HasColumnName("Category_Order");
            entity.Property(e => e.IsDefault)
                .HasColumnName("Is_Default");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => new { e.CourseId, e.Name }).IsUnique();
            entity.HasIndex(e => new { e.CourseId, e.Order });

            entity.HasOne(d => d.Course).WithMany(p => p.GradeCategories)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Grade_Categories_Course");
        });

        modelBuilder.Entity<CourseAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.ToTable("Course_Assignments");

            entity.Property(e => e.AssignmentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Assignment_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.GradeCategoryId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Grade_Category_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            entity.Property(e => e.Instructions)
                .HasMaxLength(4000);
            entity.Property(e => e.PointsPossible)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("Points_Possible");
            entity.Property(e => e.DueAt)
                .HasColumnType("datetime")
                .HasColumnName("Due_At");
            entity.Property(e => e.SubmissionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(AssignmentSubmissionType.Text)
                .HasColumnName("Submission_Type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(AssignmentStatus.Draft);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.GradeCategoryId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueAt);
            entity.HasIndex(e => new { e.CourseId, e.Status, e.DueAt });

            entity.HasOne(d => d.Course).WithMany(p => p.CourseAssignments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Assignments_Course");

            entity.HasOne(d => d.GradeCategory).WithMany(p => p.CourseAssignments)
                .HasForeignKey(d => d.GradeCategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Course_Assignments_Grade_Category");
        });

        modelBuilder.Entity<AssignmentRubricCriterion>(entity =>
        {
            entity.HasKey(e => e.CriterionId);

            entity.ToTable("Assignment_Rubric_Criteria");

            entity.Property(e => e.CriterionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Criterion_ID");
            entity.Property(e => e.AssignmentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Assignment_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(160);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.PointsPossible)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("Points_Possible");
            entity.Property(e => e.Order)
                .HasColumnName("Criterion_Order");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.AssignmentId);
            entity.HasIndex(e => new { e.AssignmentId, e.Order });

            entity.HasOne(d => d.Assignment).WithMany(p => p.RubricCriteria)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Assignment_Rubric_Criteria_Assignment");
        });

        modelBuilder.Entity<AssignmentRubricScore>(entity =>
        {
            entity.HasKey(e => e.RubricScoreId);

            entity.ToTable("Assignment_Rubric_Scores");

            entity.Property(e => e.RubricScoreId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Rubric_Score_ID");
            entity.Property(e => e.SubmissionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Submission_ID");
            entity.Property(e => e.CriterionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Criterion_ID");
            entity.Property(e => e.Score)
                .HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Feedback)
                .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.SubmissionId);
            entity.HasIndex(e => e.CriterionId);
            entity.HasIndex(e => new { e.SubmissionId, e.CriterionId }).IsUnique();

            entity.HasOne(d => d.Submission).WithMany(p => p.RubricScores)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Assignment_Rubric_Scores_Submission");

            entity.HasOne(d => d.Criterion).WithMany(p => p.Scores)
                .HasForeignKey(d => d.CriterionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Assignment_Rubric_Scores_Criterion");
        });

        modelBuilder.Entity<CourseAnnouncement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId);

            entity.ToTable("Course_Announcements");

            entity.Property(e => e.AnnouncementId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Announcement_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.CreatedByUserId)
                .HasMaxLength(450)
                .HasColumnName("Created_By_User_ID");
            entity.Property(e => e.CreatedByRole)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Created_By_Role");
            entity.Property(e => e.CreatedByName)
                .HasMaxLength(200)
                .HasColumnName("Created_By_Name");
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            entity.Property(e => e.Body)
                .HasMaxLength(4000);
            entity.Property(e => e.IsPinned)
                .HasDefaultValue(false)
                .HasColumnName("Is_Pinned");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("Is_Published");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("Published_At");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.CreatedByUserId);
            entity.HasIndex(e => e.IsPublished);
            entity.HasIndex(e => e.PublishedAt);
            entity.HasIndex(e => new { e.CourseId, e.IsPublished, e.PublishedAt });
            entity.HasIndex(e => new { e.CourseId, e.IsPinned, e.PublishedAt });

            entity.HasOne(d => d.Course).WithMany(p => p.CourseAnnouncements)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Announcements_Course");

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Course_Announcements_AppUser");
        });

        modelBuilder.Entity<CourseLiveSession>(entity =>
        {
            entity.HasKey(e => e.LiveSessionId);

            entity.ToTable("Course_Live_Sessions");

            entity.Property(e => e.LiveSessionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Live_Session_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            entity.Property(e => e.Description)
                .HasMaxLength(2000);
            entity.Property(e => e.StartsAt)
                .HasColumnType("datetime")
                .HasColumnName("Starts_At");
            entity.Property(e => e.EndsAt)
                .HasColumnType("datetime")
                .HasColumnName("Ends_At");
            entity.Property(e => e.MeetingUrl)
                .HasMaxLength(1000)
                .HasColumnName("Meeting_Url");
            entity.Property(e => e.RecordingUrl)
                .HasMaxLength(1000)
                .HasColumnName("Recording_Url");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(CourseLiveSessionStatus.Draft);
            entity.Property(e => e.AttendanceRequired)
                .HasDefaultValue(false)
                .HasColumnName("Attendance_Required");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("Published_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartsAt);
            entity.HasIndex(e => new { e.CourseId, e.Status, e.StartsAt });

            entity.HasOne(d => d.Course).WithMany(p => p.CourseLiveSessions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Live_Sessions_Course");
        });

        modelBuilder.Entity<CourseDiscussionQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId);

            entity.ToTable("Course_Discussion_Questions");

            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Question_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            entity.Property(e => e.Body)
                .HasMaxLength(4000);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(DiscussionQuestionStatus.Open);
            entity.Property(e => e.IsPinned)
                .HasDefaultValue(false)
                .HasColumnName("Is_Pinned");
            entity.Property(e => e.AcceptedAnswerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Accepted_Answer_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.Property(e => e.LastActivityAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Last_Activity_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LastActivityAt);
            entity.HasIndex(e => new { e.CourseId, e.Status, e.LastActivityAt });
            entity.HasIndex(e => new { e.CourseId, e.IsPinned, e.LastActivityAt });

            entity.HasOne(d => d.Course).WithMany(p => p.DiscussionQuestions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Discussion_Questions_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.DiscussionQuestions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Discussion_Questions_Student");
        });

        modelBuilder.Entity<CourseDiscussionAnswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId);

            entity.ToTable("Course_Discussion_Answers");

            entity.Property(e => e.AnswerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Answer_ID");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Question_ID");
            entity.Property(e => e.AuthorUserId)
                .HasMaxLength(450)
                .HasColumnName("Author_User_ID");
            entity.Property(e => e.AuthorEntityId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Author_Entity_ID");
            entity.Property(e => e.AuthorRole)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Author_Role");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(200)
                .HasColumnName("Author_Name");
            entity.Property(e => e.AuthorEmail)
                .HasMaxLength(256)
                .HasColumnName("Author_Email");
            entity.Property(e => e.Body)
                .HasMaxLength(4000);
            entity.Property(e => e.IsAccepted)
                .HasDefaultValue(false)
                .HasColumnName("Is_Accepted");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.QuestionId);
            entity.HasIndex(e => e.AuthorUserId);
            entity.HasIndex(e => e.AuthorEntityId);
            entity.HasIndex(e => e.IsAccepted);
            entity.HasIndex(e => new { e.QuestionId, e.CreatedAt });

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Discussion_Answers_Question");
        });

        modelBuilder.Entity<CourseCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId);

            entity.ToTable("Course_Certificates");

            entity.Property(e => e.CertificateId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Certificate_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.VerificationCode)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("Verification_Code");
            entity.Property(e => e.IssuedAt)
                .HasColumnType("datetime")
                .HasColumnName("Issued_At");

            entity.HasIndex(e => e.VerificationCode).IsUnique();
            entity.HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();
            entity.HasIndex(e => e.StudentId);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseCertificates)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Certificates_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseCertificates)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Certificates_Student");
        });

        modelBuilder.Entity<CourseCertificateSetting>(entity =>
        {
            entity.HasKey(e => e.CourseId);

            entity.ToTable("Course_Certificate_Settings");

            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("Is_Enabled");
            entity.Property(e => e.TemplateName)
                .HasMaxLength(80)
                .HasColumnName("Template_Name");
            entity.Property(e => e.CertificateTitle)
                .HasMaxLength(160)
                .HasColumnName("Certificate_Title");
            entity.Property(e => e.BodyText)
                .HasMaxLength(1000)
                .HasColumnName("Body_Text");
            entity.Property(e => e.AccentColor)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("Accent_Color");
            entity.Property(e => e.RequiredLecturePercent)
                .HasColumnType("decimal(5, 2)")
                .HasDefaultValue(100m)
                .HasColumnName("Required_Lecture_Percent");
            entity.Property(e => e.RequirePublishedQuizzes)
                .HasDefaultValue(true)
                .HasColumnName("Require_Published_Quizzes");
            entity.Property(e => e.RequirePublishedAssignments)
                .HasColumnName("Require_Published_Assignments");
            entity.Property(e => e.RequiredAssignmentPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Required_Assignment_Percent");
            entity.Property(e => e.MinimumFinalGradePercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Minimum_Final_Grade_Percent");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasOne(d => d.Course).WithOne(p => p.CertificateSetting)
                .HasForeignKey<CourseCertificateSetting>(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Certificate_Settings_Course");
        });

        modelBuilder.Entity<CourseModule>(entity =>
        {
            entity.HasKey(e => e.ModuleId);

            entity.ToTable("Course_Modules");

            entity.Property(e => e.ModuleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Module_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.Order)
                .HasColumnName("Module_Order");
            entity.Property(e => e.PrerequisiteModuleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Prerequisite_Module_ID");
            entity.Property(e => e.UnlockAfterDays)
                .HasColumnName("Unlock_After_Days");
            entity.Property(e => e.AvailableFrom)
                .HasColumnType("datetime")
                .HasColumnName("Available_From");
            entity.Property(e => e.AvailableUntil)
                .HasColumnType("datetime")
                .HasColumnName("Available_Until");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("Is_Published");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => new { e.CourseId, e.Order });
            entity.HasIndex(e => e.PrerequisiteModuleId);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseModules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Modules_Course");

            entity.HasOne(d => d.PrerequisiteModule).WithMany(p => p.DependentModules)
                .HasForeignKey(d => d.PrerequisiteModuleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Course_Modules_Prerequisite_Module");
        });

        modelBuilder.Entity<FileCourse>(entity =>
        {
            entity.HasOne(d => d.Course)
                .WithMany(p => p.FileCourses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_FileCourses_Course");
        });

        modelBuilder.Entity<CourseStudyGroup>(entity =>
        {
            entity.HasKey(e => e.StudyGroupId);

            entity.ToTable("Course_Study_Groups");

            entity.Property(e => e.StudyGroupId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Study_Group_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.ConversationId)
                .HasColumnName("Conversation_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(120);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.Kind)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(CourseStudyGroupKind.StudyGroup);
            entity.Property(e => e.IsDefaultCohort)
                .HasColumnName("Is_Default_Cohort");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.CreatedByUserId)
                .HasMaxLength(450)
                .HasColumnName("Created_By_User_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.ConversationId).IsUnique();
            entity.HasIndex(e => new { e.CourseId, e.IsDefaultCohort })
                .IsUnique()
                .HasFilter("[Is_Default_Cohort] = 1");
            entity.HasIndex(e => new { e.CourseId, e.IsActive });

            entity.HasOne(d => d.Course).WithMany(p => p.StudyGroups)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Study_Groups_Course");

            entity.HasOne(d => d.Conversation).WithOne()
                .HasForeignKey<CourseStudyGroup>(d => d.ConversationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Study_Groups_Conversation");
        });

        modelBuilder.Entity<CourseOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Course_O__3260905ED3772281");

            entity.ToTable("Course_Options");

            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
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

            entity.HasIndex(e => e.CourseId);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseOptions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Course_Options_Course");
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

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.PaymentStatus);
            entity.HasIndex(e => new { e.StudentId, e.PaymentStatus });
            entity.HasIndex(e => new { e.CourseId, e.PaymentStatus });

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

        modelBuilder.Entity<GymClassSession>(entity =>
        {
            entity.HasKey(e => e.ClassSessionId);

            entity.ToTable("Gym_Class_Sessions");

            entity.Property(e => e.ClassSessionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Class_Session_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");
            entity.Property(e => e.StartsAt)
                .HasColumnType("datetime")
                .HasColumnName("Starts_At");
            entity.Property(e => e.EndsAt)
                .HasColumnType("datetime")
                .HasColumnName("Ends_At");
            entity.Property(e => e.Location)
                .HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(ClassSessionStatus.Scheduled);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.TrainerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartsAt);
            entity.HasIndex(e => new { e.Status, e.StartsAt });

            entity.HasOne(d => d.Trainer).WithMany(p => p.GymClassSessions)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Gym_Class_Sessions_Trainer");
        });

        modelBuilder.Entity<ClassBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId);

            entity.ToTable("Class_Bookings");

            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Booking_ID");
            entity.Property(e => e.ClassSessionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Class_Session_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(ClassBookingStatus.Booked);
            entity.Property(e => e.BookedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Booked_At");
            entity.Property(e => e.CancelledAt)
                .HasColumnType("datetime")
                .HasColumnName("Cancelled_At");
            entity.Property(e => e.AttendanceMarkedAt)
                .HasColumnType("datetime")
                .HasColumnName("Attendance_Marked_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.ClassSessionId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.ClassSessionId, e.StudentId });
            entity.HasIndex(e => new { e.ClassSessionId, e.Status });

            entity.HasOne(d => d.ClassSession).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClassSessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Class_Bookings_Class_Session");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassBookings)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Class_Bookings_Student");
        });

        modelBuilder.Entity<WorkoutPlan>(entity =>
        {
            entity.HasKey(e => e.WorkoutPlanId);

            entity.ToTable("Workout_Plans");

            entity.Property(e => e.WorkoutPlanId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Workout_Plan_ID");
            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Goal)
                .HasMaxLength(500);
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DurationWeeks)
                .HasColumnName("Duration_Weeks");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.TrainerId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.TrainerId, e.IsActive });

            entity.HasOne(d => d.Trainer).WithMany(p => p.WorkoutPlans)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Workout_Plans_Trainer");
        });

        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(e => e.ExerciseId);

            entity.ToTable("Workout_Exercises");

            entity.Property(e => e.ExerciseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Exercise_ID");
            entity.Property(e => e.WorkoutPlanId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Workout_Plan_ID");
            entity.Property(e => e.WeekNumber)
                .HasColumnName("Week_Number");
            entity.Property(e => e.DayNumber)
                .HasColumnName("Day_Number");
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Reps)
                .HasMaxLength(80);
            entity.Property(e => e.RestSeconds)
                .HasColumnName("Rest_Seconds");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);

            entity.HasIndex(e => e.WorkoutPlanId);
            entity.HasIndex(e => new { e.WorkoutPlanId, e.WeekNumber, e.DayNumber, e.Order });

            entity.HasOne(d => d.WorkoutPlan).WithMany(p => p.Exercises)
                .HasForeignKey(d => d.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Workout_Exercises_Workout_Plan");
        });

        modelBuilder.Entity<StudentWorkoutAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.ToTable("Student_Workout_Assignments");

            entity.Property(e => e.AssignmentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Assignment_ID");
            entity.Property(e => e.WorkoutPlanId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Workout_Plan_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.TrainerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Trainer_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(WorkoutAssignmentStatus.Active);
            entity.Property(e => e.StartsAt)
                .HasColumnType("datetime")
                .HasColumnName("Starts_At");
            entity.Property(e => e.EndsAt)
                .HasColumnType("datetime")
                .HasColumnName("Ends_At");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("Completed_At");
            entity.Property(e => e.CancelledAt)
                .HasColumnType("datetime")
                .HasColumnName("Cancelled_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.WorkoutPlanId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.TrainerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.StudentId, e.Status });
            entity.HasIndex(e => new { e.TrainerId, e.Status });

            entity.HasOne(d => d.WorkoutPlan).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Student_Workout_Assignments_Workout_Plan");

            entity.HasOne(d => d.Student).WithMany(p => p.WorkoutAssignments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Student_Workout_Assignments_Student");

            entity.HasOne(d => d.Trainer).WithMany(p => p.WorkoutAssignments)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Student_Workout_Assignments_Trainer");
        });

        modelBuilder.Entity<StudentProgressLog>(entity =>
        {
            entity.HasKey(e => e.ProgressLogId);

            entity.ToTable("Student_Progress_Logs");

            entity.Property(e => e.ProgressLogId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Progress_Log_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.LoggedAt)
                .HasColumnType("datetime")
                .HasColumnName("Logged_At");
            entity.Property(e => e.WeightKg)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Weight_Kg");
            entity.Property(e => e.BodyFatPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Body_Fat_Percent");
            entity.Property(e => e.WaistCm)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Waist_Cm");
            entity.Property(e => e.ChestCm)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Chest_Cm");
            entity.Property(e => e.ArmCm)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Arm_Cm");
            entity.Property(e => e.HipCm)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Hip_Cm");
            entity.Property(e => e.StrengthNotes)
                .HasMaxLength(1000)
                .HasColumnName("Strength_Notes");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.LoggedAt);
            entity.HasIndex(e => new { e.StudentId, e.LoggedAt });

            entity.HasOne(d => d.Student).WithMany(p => p.ProgressLogs)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Student_Progress_Logs_Student");
        });

        modelBuilder.Entity<StudentProgressGoal>(entity =>
        {
            entity.HasKey(e => e.ProgressGoalId);

            entity.ToTable("Student_Progress_Goals");

            entity.Property(e => e.ProgressGoalId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Progress_Goal_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.TargetWeightKg)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Target_Weight_Kg");
            entity.Property(e => e.TargetBodyFatPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Target_Body_Fat_Percent");
            entity.Property(e => e.GoalDate)
                .HasColumnType("datetime")
                .HasColumnName("Goal_Date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(ProgressGoalStatus.Active);
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.StudentId, e.Status });

            entity.HasOne(d => d.Student).WithMany(p => p.ProgressGoals)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Student_Progress_Goals_Student");
        });

        modelBuilder.Entity<WorkoutExerciseCompletion>(entity =>
        {
            entity.HasKey(e => e.CompletionId);

            entity.ToTable("Workout_Exercise_Completions");

            entity.Property(e => e.CompletionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Completion_ID");
            entity.Property(e => e.AssignmentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Assignment_ID");
            entity.Property(e => e.ExerciseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Exercise_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.CompletedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Completed_At");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);

            entity.HasIndex(e => e.AssignmentId);
            entity.HasIndex(e => e.ExerciseId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => new { e.AssignmentId, e.ExerciseId }).IsUnique();

            entity.HasOne(d => d.Assignment).WithMany(p => p.Completions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Workout_Exercise_Completions_Assignment");

            entity.HasOne(d => d.Exercise).WithMany(p => p.Completions)
                .HasForeignKey(d => d.ExerciseId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Workout_Exercise_Completions_Exercise");

            entity.HasOne(d => d.Student).WithMany(p => p.WorkoutExerciseCompletions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Workout_Exercise_Completions_Student");
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
            entity.Property(e => e.ModuleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Module_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Activity_Type")
                .HasDefaultValue(LearningActivityType.Lesson);
            entity.Property(e => e.Order)
                .HasColumnName("Lecture_Order");
            entity.Property(e => e.PrerequisiteLectureId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Prerequisite_Lecture_ID");
            entity.Property(e => e.UnlockAfterDays)
                .HasColumnName("Unlock_After_Days");
            entity.Property(e => e.AvailableFrom)
                .HasColumnType("datetime")
                .HasColumnName("Available_From");
            entity.Property(e => e.AvailableUntil)
                .HasColumnType("datetime")
                .HasColumnName("Available_Until");
            entity.Property(e => e.IsPreview)
                .HasDefaultValue(false)
                .HasColumnName("Is_Preview");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("Is_Published");

            entity.HasIndex(e => e.ModuleId);
            entity.HasIndex(e => new { e.CourseId, e.ModuleId, e.Order });
            entity.HasIndex(e => e.PrerequisiteLectureId);

            entity.HasOne(d => d.Course).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Lectures_Course");

            entity.HasOne(d => d.Module).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Lectures_Course_Module");

            entity.HasOne(d => d.PrerequisiteLecture).WithMany(p => p.DependentLectures)
                .HasForeignKey(d => d.PrerequisiteLectureId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Lectures_Prerequisite_Lecture");
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

        modelBuilder.Entity<LectureProgress>(entity =>
        {
            entity.HasKey(e => e.LectureProgressId).HasName("PK_Lecture_Progress");

            entity.ToTable("Lecture_Progress");

            entity.Property(e => e.LectureProgressId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lecture_Progress_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.LectureId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lecture_ID");
            entity.Property(e => e.IsCompleted).HasColumnName("Is_Completed");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("Completed_At");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => new { e.StudentId, e.LectureId }).IsUnique();
            entity.HasIndex(e => e.LectureId);

            entity.HasOne(d => d.Lecture).WithMany(p => p.LectureProgresses)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Lecture_Progress_Lecture");

            entity.HasOne(d => d.Student).WithMany(p => p.LectureProgresses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Lecture_Progress_Student");
        });

        modelBuilder.Entity<CourseQuiz>(entity =>
        {
            entity.HasKey(e => e.QuizId);

            entity.ToTable("Course_Quizzes");

            entity.Property(e => e.QuizId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quiz_ID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Course_ID");
            entity.Property(e => e.ModuleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Module_ID");
            entity.Property(e => e.LectureId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lecture_ID");
            entity.Property(e => e.GradeCategoryId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Grade_Category_ID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ScopeType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(AssessmentScopeType.Course)
                .HasColumnName("Scope_Type");
            entity.Property(e => e.PassingScorePercent).HasColumnName("Passing_Score_Percent");
            entity.Property(e => e.TimeLimitMinutes).HasColumnName("Time_Limit_Minutes");
            entity.Property(e => e.MaxAttempts).HasColumnName("Max_Attempts");
            entity.Property(e => e.ShuffleQuestions).HasColumnName("Shuffle_Questions");
            entity.Property(e => e.ShuffleOptions).HasColumnName("Shuffle_Options");
            entity.Property(e => e.ShowCorrectAnswers).HasColumnName("Show_Correct_Answers");
            entity.Property(e => e.RequireHonorCode).HasColumnName("Require_Honor_Code");
            entity.Property(e => e.TrackProctoringSignals).HasColumnName("Track_Proctoring_Signals");
            entity.Property(e => e.AvailableFrom)
                .HasColumnType("datetime")
                .HasColumnName("Available_From");
            entity.Property(e => e.AvailableUntil)
                .HasColumnType("datetime")
                .HasColumnName("Available_Until");
            entity.Property(e => e.IsPublished).HasColumnName("Is_Published");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.ModuleId);
            entity.HasIndex(e => e.LectureId);
            entity.HasIndex(e => e.GradeCategoryId);
            entity.HasIndex(e => new { e.CourseId, e.ScopeType });

            entity.HasOne(d => d.Course).WithMany(p => p.CourseQuizzes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Course_Quizzes_Course");

            entity.HasOne(d => d.Module).WithMany()
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Course_Quizzes_Course_Module");

            entity.HasOne(d => d.Lecture).WithMany()
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Course_Quizzes_Lecture");

            entity.HasOne(d => d.GradeCategory).WithMany(p => p.CourseQuizzes)
                .HasForeignKey(d => d.GradeCategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Course_Quizzes_Grade_Category");
        });

        modelBuilder.Entity<QuizQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId);

            entity.ToTable("Quiz_Questions");

            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Question_ID");
            entity.Property(e => e.QuizId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quiz_ID");
            entity.Property(e => e.Prompt).HasColumnType("nvarchar(max)");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(QuizQuestionType.SingleChoice)
                .HasColumnName("Question_Type");
            entity.Property(e => e.Explanation).HasColumnType("nvarchar(max)");
            entity.Property(e => e.QuestionBank)
                .HasMaxLength(120)
                .HasColumnName("Question_Bank");
            entity.Property(e => e.RequiresManualGrading).HasColumnName("Requires_Manual_Grading");

            entity.HasIndex(e => e.QuizId);
            entity.HasIndex(e => new { e.QuizId, e.Order }).IsUnique();

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quiz_Questions_Course_Quiz");
        });

        modelBuilder.Entity<QuizOption>(entity =>
        {
            entity.HasKey(e => e.OptionId);

            entity.ToTable("Quiz_Options");

            entity.Property(e => e.OptionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Option_ID");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Question_ID");
            entity.Property(e => e.Text).HasColumnType("nvarchar(max)");
            entity.Property(e => e.IsCorrect).HasColumnName("Is_Correct");

            entity.HasIndex(e => e.QuestionId);

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quiz_Options_Quiz_Question");
        });

        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasKey(e => e.AttemptId);

            entity.ToTable("Quiz_Attempts");

            entity.Property(e => e.AttemptId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Attempt_ID");
            entity.Property(e => e.QuizId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quiz_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.AttemptNumber).HasColumnName("Attempt_Number");
            entity.Property(e => e.ScorePercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Score_Percent");
            entity.Property(e => e.TotalPoints).HasColumnName("Total_Points");
            entity.Property(e => e.SubmittedAt)
                .HasColumnType("datetime")
                .HasColumnName("Submitted_At");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(QuizAttemptStatus.Submitted);
            entity.Property(e => e.RequiresManualGrading).HasColumnName("Requires_Manual_Grading");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("Started_At");
            entity.Property(e => e.QuestionOrderSnapshot)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Question_Order_Snapshot");
            entity.Property(e => e.BrowserFingerprint)
                .HasMaxLength(256)
                .HasColumnName("Browser_Fingerprint");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(64)
                .HasColumnName("Ip_Address");
            entity.Property(e => e.HonorCodeAccepted).HasColumnName("Honor_Code_Accepted");
            entity.Property(e => e.ProctoringReviewRequired).HasColumnName("Proctoring_Review_Required");
            entity.Property(e => e.FocusLostCount).HasColumnName("Focus_Lost_Count");
            entity.Property(e => e.PasteEventCount).HasColumnName("Paste_Event_Count");
            entity.Property(e => e.FullscreenExitCount).HasColumnName("Fullscreen_Exit_Count");
            entity.Property(e => e.ProctoringEventCount).HasColumnName("Proctoring_Event_Count");
            entity.Property(e => e.SuspiciousActivityScore).HasColumnName("Suspicious_Activity_Score");
            entity.Property(e => e.ProctoringFlags)
                .HasMaxLength(1000)
                .HasColumnName("Proctoring_Flags");
            entity.Property(e => e.GradedAt)
                .HasColumnType("datetime")
                .HasColumnName("Graded_At");
            entity.Property(e => e.Feedback).HasMaxLength(2000);

            entity.HasIndex(e => e.QuizId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => new { e.QuizId, e.StudentId, e.SubmittedAt });

            entity.HasOne(d => d.Quiz).WithMany(p => p.Attempts)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quiz_Attempts_Course_Quiz");

            entity.HasOne(d => d.Student).WithMany(p => p.QuizAttempts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quiz_Attempts_Student");
        });

        modelBuilder.Entity<QuizAttemptAnswer>(entity =>
        {
            entity.HasKey(e => e.AttemptAnswerId);

            entity.ToTable("Quiz_Attempt_Answers");

            entity.Property(e => e.AttemptAnswerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Attempt_Answer_ID");
            entity.Property(e => e.AttemptId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Attempt_ID");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Question_ID");
            entity.Property(e => e.SelectedOptionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Selected_Option_ID");
            entity.Property(e => e.SelectedOptionIds)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Selected_Option_IDs");
            entity.Property(e => e.TextAnswer)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Text_Answer");
            entity.Property(e => e.IsCorrect).HasColumnName("Is_Correct");
            entity.Property(e => e.PointsAwarded).HasColumnName("Points_Awarded");
            entity.Property(e => e.Feedback).HasMaxLength(2000);

            entity.HasIndex(e => e.AttemptId);
            entity.HasIndex(e => e.QuestionId);
            entity.HasIndex(e => e.SelectedOptionId);

            entity.HasOne(d => d.Attempt).WithMany(p => p.Answers)
                .HasForeignKey(d => d.AttemptId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quiz_Attempt_Answers_Quiz_Attempt");

            entity.HasOne(d => d.Question).WithMany(p => p.AttemptAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Quiz_Attempt_Answers_Quiz_Question");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.AttemptAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Quiz_Attempt_Answers_Quiz_Option");
        });

        modelBuilder.Entity<AssignmentSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);

            entity.ToTable("Assignment_Submissions");

            entity.Property(e => e.SubmissionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Submission_ID");
            entity.Property(e => e.AssignmentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Assignment_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.TextResponse)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Text_Response");
            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(1000)
                .HasColumnName("Attachment_Url");
            entity.Property(e => e.Score)
                .HasColumnType("decimal(8, 2)");
            entity.Property(e => e.ScorePercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Score_Percent");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(AssignmentSubmissionStatus.Submitted);
            entity.Property(e => e.Feedback)
                .HasMaxLength(2000);
            entity.Property(e => e.SimilarityScorePercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Similarity_Score_Percent");
            entity.Property(e => e.SimilarityMatchedSubmissionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Similarity_Matched_Submission_ID");
            entity.Property(e => e.SimilarityMatchedStudentName)
                .HasMaxLength(200)
                .HasColumnName("Similarity_Matched_Student_Name");
            entity.Property(e => e.SimilarityFlags)
                .HasMaxLength(1000)
                .HasColumnName("Similarity_Flags");
            entity.Property(e => e.SimilarityCheckedAt)
                .HasColumnType("datetime")
                .HasColumnName("Similarity_Checked_At");
            entity.Property(e => e.SubmittedAt)
                .HasColumnType("datetime")
                .HasColumnName("Submitted_At");
            entity.Property(e => e.GradedAt)
                .HasColumnType("datetime")
                .HasColumnName("Graded_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.AssignmentId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Assignment_Submissions_Assignment");

            entity.HasOne(d => d.Student).WithMany(p => p.AssignmentSubmissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Assignment_Submissions_Student");
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

        modelBuilder.Entity<MembershipPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId);

            entity.ToTable("Membership_Plans");

            entity.Property(e => e.PlanId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Plan_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.DurationDays)
                .HasColumnName("Duration_Days");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("Is_Active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.IsActive, e.Price });
        });

        modelBuilder.Entity<StudentMembership>(entity =>
        {
            entity.HasKey(e => e.MembershipId);

            entity.ToTable("Student_Memberships");

            entity.Property(e => e.MembershipId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Membership_ID");
            entity.Property(e => e.PlanId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Plan_ID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartsAt)
                .HasColumnType("datetime")
                .HasColumnName("Starts_At");
            entity.Property(e => e.EndsAt)
                .HasColumnType("datetime")
                .HasColumnName("Ends_At");
            entity.Property(e => e.CancelledAt)
                .HasColumnType("datetime")
                .HasColumnName("Cancelled_At");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");

            entity.HasIndex(e => e.PlanId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.EndsAt);
            entity.HasIndex(e => new { e.StudentId, e.Status, e.EndsAt });

            entity.HasOne(d => d.Plan).WithMany(p => p.StudentMemberships)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Student_Memberships_Membership_Plan");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentMemberships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Student_Memberships_Student");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // The badge/unread queries always filter by (UserId, IsRead).
            entity.HasIndex(e => new { e.UserId, e.IsRead });

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notifications_AspNetUsers");
        });

        modelBuilder.Entity<NotificationPreference>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.InAppEnabled).HasDefaultValue(true);
            entity.Property(e => e.EmailEnabled).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasIndex(e => new { e.UserId, e.Type }).IsUnique();

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NotificationPreferences_AspNetUsers");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.SubjectTemplate).HasMaxLength(250);
            entity.Property(e => e.BodyTemplate).HasMaxLength(4000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedById).HasMaxLength(450);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasIndex(e => e.Type).IsUnique();
        });

        modelBuilder.Entity<NotificationDeliveryLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.RecipientEmail).HasMaxLength(256);
            entity.Property(e => e.RecipientName).HasMaxLength(450);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Channel).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.Subject).HasMaxLength(250);
            entity.Property(e => e.Content).HasMaxLength(4000);
            entity.Property(e => e.Link).HasMaxLength(1000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => new { e.Channel, e.Status });
            entity.HasIndex(e => e.UserId);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NotificationDeliveryLogs_AspNetUsers");

            entity.HasOne(d => d.Notification).WithMany()
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_NotificationDeliveryLogs_Notifications");
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

            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.PaymentStatus);
            entity.HasIndex(e => e.PaymentType);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.CourseId, e.PaymentStatus, e.CreatedAt });
            entity.HasIndex(e => new { e.StudentId, e.PaymentStatus, e.CreatedAt });
            entity.HasIndex(e => new { e.PaymentType, e.CreatedAt });

            entity.HasOne(d => d.Course).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Payments_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Payments_Student");
        });

        modelBuilder.Entity<PaymentEvent>(entity =>
        {
            entity.HasKey(e => e.PaymentEventId);

            entity.ToTable("Payment_Events");

            entity.Property(e => e.PaymentEventId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Event_ID");
            entity.Property(e => e.PaymentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_ID");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Event_Type");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Old_Status");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("New_Status");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Message)
                .HasMaxLength(1000);
            entity.Property(e => e.RawPayload)
                .HasColumnName("Raw_Payload");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.Source);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.PaymentId, e.CreatedAt });

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentEvents)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payment_Events_Payment");
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
            entity.Property(e => e.HealthStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Health_Status");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Profile_Picture");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.HealthStatus);
            entity.HasIndex(e => new { e.HealthStatus, e.CreatedAt });
            entity.HasOne(d => d.AppUser).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .HasConstraintName("FK_Student_AppUser");
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
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue(TrainerApprovalStatus.PendingReview);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
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
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Certification);
            entity.HasIndex(e => e.ApprovalStatus);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.Category, e.Experience });
            entity.HasIndex(e => new { e.Category, e.Rating });
            entity.HasOne(d => d.AppUser).WithOne(p => p.Trainer)
                .HasForeignKey<Trainer>(d => d.UserId)
                .HasConstraintName("FK_Trainer_AppUser");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.Property(e => e.IsGroup).HasDefaultValue(false);
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue(ParticipantRoles.Member);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Type).HasMaxLength(20).HasDefaultValue(MessageTypes.Text);
        });

        modelBuilder.Entity<FoodNutrition>(entity =>
        {
            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<FoodNutritionUser>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.Date });
            // SetNull: deleting a master food keeps the user's logged snapshot.
            entity.HasOne<FoodNutrition>().WithMany()
                .HasForeignKey(e => e.FoodNutritionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<NutritionBudget>(entity =>
        {
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        modelBuilder.SeedData();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
