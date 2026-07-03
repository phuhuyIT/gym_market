using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseAnalytics;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseAnalyticsIntegrationTests : BaseIntegrationTests
{
    public CourseAnalyticsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CourseDashboard_SummarizesLearnerProgressAndRisk()
    {
        var courseId = await CreateTrainerCourseAsync("analytics_trainer@example.com");

        await AuthenticateAsync(email: "analytics_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedCourseActivityAsync(courseId, studentId, includeProgress: true);

        await AuthenticateAsync(email: "analytics_trainer@example.com", role: "Trainer");
        var dashboard = await Client.GetFromJsonAsync<CourseAnalyticsDashboardDto>($"/api/CourseAnalytics/course/{courseId}");

        Assert.NotNull(dashboard);
        Assert.Equal(courseId, dashboard!.CourseId);
        Assert.Equal(1, dashboard.TotalLearners);
        Assert.Equal(2, dashboard.TotalLectures);
        Assert.Equal(1, dashboard.TotalAssignments);
        Assert.Equal(2, dashboard.TotalGradeItems);
        Assert.Equal(50, dashboard.AverageLessonProgressPercent);
        Assert.Equal(75, dashboard.AverageFinalGradePercent);

        var learner = Assert.Single(dashboard.Learners);
        Assert.Equal(studentId, learner.StudentId);
        Assert.Equal(1, learner.CompletedLectures);
        Assert.Equal(1, learner.SubmittedAssignments);
        Assert.Equal(2, learner.CompletedGradeItems);
        Assert.False(learner.IsAtRisk);
    }

    [Fact]
    public async Task MyCourseProgress_RequiresPaidCourseAccess()
    {
        var courseId = await CreateTrainerCourseAsync("analytics_access_trainer@example.com");

        await AuthenticateAsync(email: "analytics_paid_student@example.com", role: "Student");
        var paidStudentId = GetTokenClaim("studentId")!;
        await SeedCourseActivityAsync(courseId, paidStudentId, includeProgress: false);

        var progress = await Client.GetFromJsonAsync<MyCourseAnalyticsDto>($"/api/CourseAnalytics/course/{courseId}/me");
        Assert.NotNull(progress);
        Assert.Equal(paidStudentId, progress!.Progress.StudentId);

        await AuthenticateAsync(email: "analytics_unpaid_student@example.com", role: "Student");
        var forbidden = await Client.GetAsync($"/api/CourseAnalytics/course/{courseId}/me");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    private async Task<string> CreateTrainerCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "ANALYTICS_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Analytics " + courseId,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 100,
            MaxParticipants = 10,
            Duration = 10,
            Status = CourseStatus.PendingReview
        });
        response.EnsureSuccessStatusCode();
        return courseId;
    }

    private async Task SeedCourseActivityAsync(string courseId, string studentId, bool includeProgress)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var now = DateTime.UtcNow;

        db.Payments.Add(new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Paid,
            CreatedAt = now
        });

        var firstLecture = new Lecture
        {
            LectureId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Warm up",
            Order = 1,
            IsPublished = true
        };
        var secondLecture = new Lecture
        {
            LectureId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Main set",
            Order = 2,
            IsPublished = true
        };
        db.Lectures.AddRange(firstLecture, secondLecture);

        if (includeProgress)
        {
            db.LectureProgresses.Add(new LectureProgress
            {
                LectureProgressId = Guid.NewGuid().ToString(),
                StudentId = studentId,
                LectureId = firstLecture.LectureId,
                IsCompleted = true,
                CompletedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        var assignment = new CourseAssignment
        {
            AssignmentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Training reflection",
            PointsPossible = 100,
            Status = AssignmentStatus.Published,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.CourseAssignments.Add(assignment);
        db.AssignmentSubmissions.Add(new AssignmentSubmission
        {
            SubmissionId = Guid.NewGuid().ToString(),
            AssignmentId = assignment.AssignmentId,
            StudentId = studentId,
            TextResponse = "Done",
            Score = 80,
            ScorePercent = 80,
            Status = AssignmentSubmissionStatus.Graded,
            SubmittedAt = now,
            GradedAt = now,
            UpdatedAt = now
        });

        var quiz = new CourseQuiz
        {
            QuizId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Checkpoint",
            IsPublished = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.CourseQuizzes.Add(quiz);
        db.QuizAttempts.Add(new QuizAttempt
        {
            AttemptId = Guid.NewGuid().ToString(),
            QuizId = quiz.QuizId,
            StudentId = studentId,
            Score = 70,
            TotalPoints = 100,
            ScorePercent = 70,
            Status = QuizAttemptStatus.Graded,
            SubmittedAt = now
        });

        await db.SaveChangesAsync();
    }
}
