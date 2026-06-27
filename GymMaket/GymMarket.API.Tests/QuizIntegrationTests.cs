using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Quiz;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class QuizIntegrationTests : BaseIntegrationTests
{
    public QuizIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetQuiz_AsUnpaidStudent_ReturnsForbidden()
    {
        var courseId = await CreateCourseWithQuizAsync("quiz_unpaid_trainer@example.com");

        await AuthenticateAsync(email: "quiz_unpaid_student@example.com", role: "Student");
        var response = await Client.GetAsync($"/api/Quiz/course/{courseId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SubmitQuiz_AsPaidStudent_ScoresAttempt()
    {
        var courseId = await CreateCourseWithQuizAsync("quiz_paid_trainer@example.com");

        await AuthenticateAsync(email: "quiz_paid_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var quiz = await Client.GetFromJsonAsync<CourseQuizDto>($"/api/Quiz/course/{courseId}");
        Assert.NotNull(quiz);

        var correctOptionId = quiz!.Questions[0].Options[0].OptionId;
        var response = await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}/submit", new SubmitQuizAttemptDto
        {
            Answers =
            [
                new SubmitQuizAnswerDto
                {
                    QuestionId = quiz.Questions[0].QuestionId,
                    SelectedOptionId = correctOptionId
                }
            ]
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var attempt = await response.Content.ReadFromJsonAsync<QuizAttemptSummaryDto>();
        Assert.NotNull(attempt);
        Assert.Equal(1, attempt!.Score);
        Assert.Equal(1, attempt.TotalPoints);
        Assert.Equal(100, attempt.ScorePercent);
        Assert.True(attempt.Passed);
    }

    [Fact]
    public async Task Gradebook_AsOwningTrainer_ReturnsAttempts()
    {
        var courseId = await CreateCourseWithQuizAsync("quiz_gradebook_trainer@example.com");

        await AuthenticateAsync(email: "quiz_gradebook_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);
        var quiz = await Client.GetFromJsonAsync<CourseQuizDto>($"/api/Quiz/course/{courseId}");
        await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}/submit", new SubmitQuizAttemptDto
        {
            Answers =
            [
                new SubmitQuizAnswerDto
                {
                    QuestionId = quiz!.Questions[0].QuestionId,
                    SelectedOptionId = quiz.Questions[0].Options[0].OptionId
                }
            ]
        });

        await AuthenticateAsync(email: "quiz_gradebook_trainer@example.com", role: "Trainer");
        var gradebook = await Client.GetFromJsonAsync<List<QuizAttemptSummaryDto>>($"/api/Quiz/course/{courseId}/gradebook");

        Assert.NotNull(gradebook);
        Assert.Single(gradebook!);
        Assert.Equal(100, gradebook[0].ScorePercent);
    }

    private async Task<string> CreateCourseWithQuizAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "QUIZ_COURSE_" + Guid.NewGuid().ToString("N")[..8];
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
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

        var response = await Client.PutAsJsonAsync($"/api/Quiz/course/{courseId}", new UpsertCourseQuizDto
        {
            Title = "Final check",
            PassingScorePercent = 70,
            IsPublished = true,
            Questions =
            [
                new UpsertQuizQuestionDto
                {
                    Prompt = "What should you do before training?",
                    Order = 1,
                    Points = 1,
                    Options =
                    [
                        new UpsertQuizOptionDto { Text = "Warm up", IsCorrect = true },
                        new UpsertQuizOptionDto { Text = "Skip hydration", IsCorrect = false }
                    ]
                }
            ]
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
