using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GymMarket.API.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    [JsonIgnore]
    public string? Password { get; set; }

    public string? HealthStatus { get; set; }

    public string? ProfilePicture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual ICollection<ClassBooking> ClassBookings { get; set; } = new List<ClassBooking>();

    public virtual ICollection<CourseCertificate> CourseCertificates { get; set; } = new List<CourseCertificate>();

    public virtual ICollection<HealthDatum> HealthData { get; set; } = new List<HealthDatum>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<LectureProgress> LectureProgresses { get; set; } = new List<LectureProgress>();

    public virtual ICollection<StudentMembership> StudentMemberships { get; set; } = new List<StudentMembership>();

    public virtual ICollection<StudentProgressLog> ProgressLogs { get; set; } = new List<StudentProgressLog>();

    public virtual ICollection<StudentProgressGoal> ProgressGoals { get; set; } = new List<StudentProgressGoal>();

    public virtual ICollection<StudentWorkoutAssignment> WorkoutAssignments { get; set; } = new List<StudentWorkoutAssignment>();

    public virtual ICollection<WorkoutExerciseCompletion> WorkoutExerciseCompletions { get; set; } = new List<WorkoutExerciseCompletion>();

    public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();

    public virtual AppUser? AppUser { get; set; }
}
