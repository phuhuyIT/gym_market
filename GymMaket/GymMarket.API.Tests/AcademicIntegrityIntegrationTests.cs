using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Assignments;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Quiz;
using GymMarket.API.Models;
using GymMarket.API.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class AcademicIntegrityIntegrationTests : BaseIntegrationTests
{
    public AcademicIntegrityIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task AssignmentSubmission_ComputesSimilarityAgainstPriorSubmissions()
    {
        var courseId = await CreateCourseAsync("integrity_assignment_trainer@example.com", "INTEGRITY_ASSIGNMENT");
        var created = await Client.PostAsJsonAsync($"/api/Assignments/course/{courseId}", new UpsertCourseAssignmentDto
        {
            Title = "Program design reflection",
            PointsPossible = 20,
            SubmissionType = AssignmentSubmissionType.Text,
            Status = AssignmentStatus.Published
        });
        var assignment = await created.Content.ReadFromJsonAsync<CourseAssignmentDto>();

        const string responseText = "Progressive overload should be applied gradually across multiple weeks while monitoring recovery, sleep, hydration, soreness, and exercise technique so the client can adapt without unnecessary injury risk.";

        await AuthenticateAsync(email: "integrity_assignment_student_a@example.com", role: "Student");
        var studentA = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentA, courseId);
        var first = await Client.PostAsJsonAsync($"/api/Assignments/{assignment!.AssignmentId}/submit", new SubmitAssignmentDto
        {
            TextResponse = responseText
        });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        await AuthenticateAsync(email: "integrity_assignment_student_b@example.com", role: "Student");
        var studentB = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentB, courseId);
        var second = await Client.PostAsJsonAsync($"/api/Assignments/{assignment.AssignmentId}/submit", new SubmitAssignmentDto
        {
            TextResponse = responseText
        });

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var submission = await second.Content.ReadFromJsonAsync<AssignmentSubmissionDto>();
        Assert.NotNull(submission);
        Assert.True(submission!.SimilarityScorePercent >= 85);
        Assert.NotNull(submission.SimilarityMatchedSubmissionId);
        Assert.Contains("High similarity", submission.SimilarityFlags);
    }

    [Fact]
    public async Task QuizSubmission_RequiresHonorCodeAndStoresProctoringSignals()
    {
        var courseId = await CreateCourseAsync("integrity_quiz_trainer@example.com", "INTEGRITY_QUIZ");
        var quizCreate = await Client.PutAsJsonAsync($"/api/Quiz/course/{courseId}", new UpsertCourseQuizDto
        {
            Title = "Integrity quiz",
            PassingScorePercent = 70,
            MaxAttempts = 1,
            TimeLimitMinutes = 30,
            ShuffleQuestions = true,
            ShuffleOptions = true,
            RequireHonorCode = true,
            TrackProctoringSignals = true,
            IsPublished = true,
            Questions =
            [
                new UpsertQuizQuestionDto
                {
                    Prompt = "Which habit improves safe training?",
                    QuestionBank = "Safety",
                    Order = 1,
                    Points = 1,
                    Options =
                    [
                        new UpsertQuizOptionDto { Text = "Warm up", IsCorrect = true },
                        new UpsertQuizOptionDto { Text = "Ignore pain", IsCorrect = false }
                    ]
                }
            ]
        });
        Assert.Equal(HttpStatusCode.OK, quizCreate.StatusCode);

        await AuthenticateAsync(email: "integrity_quiz_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);
        var quiz = await Client.GetFromJsonAsync<CourseQuizDto>($"/api/Quiz/course/{courseId}");
        Assert.NotNull(quiz);
        Assert.True(quiz!.RequireHonorCode);
        Assert.True(quiz.TrackProctoringSignals);
        Assert.True(quiz.ShuffleOptions);

        var noHonor = await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}/submit", new SubmitQuizAttemptDto
        {
            StartedAt = DateTime.UtcNow,
            HonorCodeAccepted = false,
            Answers =
            [
                new SubmitQuizAnswerDto
                {
                    QuestionId = quiz.Questions[0].QuestionId,
                    SelectedOptionId = quiz.Questions[0].Options[0].OptionId
                }
            ]
        });
        Assert.Equal(HttpStatusCode.BadRequest, noHonor.StatusCode);

        var submit = await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}/submit", new SubmitQuizAttemptDto
        {
            StartedAt = DateTime.UtcNow,
            HonorCodeAccepted = true,
            BrowserFingerprint = "test-browser",
            ProctoringSignals =
            [
                new QuizProctoringSignalDto { Type = "focus_lost", Count = 4 },
                new QuizProctoringSignalDto { Type = "paste", Count = 1 }
            ],
            Answers =
            [
                new SubmitQuizAnswerDto
                {
                    QuestionId = quiz.Questions[0].QuestionId,
                    SelectedOptionId = quiz.Questions[0].Options[0].OptionId
                }
            ]
        });

        Assert.Equal(HttpStatusCode.OK, submit.StatusCode);
        var attempt = await submit.Content.ReadFromJsonAsync<QuizAttemptSummaryDto>();
        Assert.NotNull(attempt);
        Assert.True(attempt!.HonorCodeAccepted);
        Assert.Equal(4, attempt.FocusLostCount);
        Assert.Equal(1, attempt.PasteEventCount);
        Assert.True(attempt.ProctoringReviewRequired);
        Assert.Contains("focus", attempt.ProctoringFlags);

        await AuthenticateAsync(email: "integrity_quiz_trainer@example.com", role: "Trainer");
        var gradebook = await Client.GetFromJsonAsync<List<QuizAttemptSummaryDto>>($"/api/Quiz/course/{courseId}/gradebook");
        var trainerAttempt = Assert.Single(gradebook!);
        Assert.True(trainerAttempt.ProctoringReviewRequired);
        Assert.Equal(attempt.SuspiciousActivityScore, trainerAttempt.SuspiciousActivityScore);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail, string prefix)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = prefix + "_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = courseId,
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

    private async Task SeedPaidPaymentAsync(string studentId, string courseId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        db.Payments.Add(new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Paid,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
