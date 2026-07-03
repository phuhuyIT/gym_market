using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Gradebook;
using GymMarket.API.DTOs.Quiz;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class GradebookIntegrationTests : BaseIntegrationTests
{
    public GradebookIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GradebookPolicy_WeightsCategoriesAndExportsCsv()
    {
        var courseId = await CreateCourseWithTwoQuizzesAsync("gradebook_trainer@example.com");

        var policy = await Client.GetFromJsonAsync<GradebookPolicyDto>($"/api/Gradebook/course/{courseId}/policy");
        Assert.NotNull(policy);
        Assert.Equal(2, policy!.Items.Count);
        Assert.Single(policy.Categories);
        Assert.Equal(100, policy.Categories[0].WeightPercent);

        var practiceId = Guid.NewGuid().ToString();
        var finalId = Guid.NewGuid().ToString();
        var saveResponse = await Client.PutAsJsonAsync($"/api/Gradebook/course/{courseId}/policy", new UpdateGradebookPolicyDto
        {
            Categories =
            [
                new UpdateGradeCategoryDto { CategoryId = practiceId, Name = "Practice", WeightPercent = 40, Order = 1 },
                new UpdateGradeCategoryDto { CategoryId = finalId, Name = "Final", WeightPercent = 60, Order = 2 }
            ],
            Items =
            [
                new UpdateGradeItemCategoryDto { ItemId = policy.Items[0].ItemId, CategoryId = practiceId },
                new UpdateGradeItemCategoryDto { ItemId = policy.Items[1].ItemId, CategoryId = finalId }
            ]
        });
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

        await AuthenticateAsync(email: "gradebook_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

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

        var myGrades = await Client.GetFromJsonAsync<MyCourseGradesResponse>($"/api/Gradebook/course/{courseId}/me");
        Assert.NotNull(myGrades);
        Assert.Equal(100, myGrades!.Grade.CurrentPercent);
        Assert.Equal(40, myGrades.Grade.FinalPercent);
        Assert.Equal("F", myGrades.Grade.LetterGrade);

        await AuthenticateAsync(email: "gradebook_trainer@example.com", role: "Trainer");
        var gradebook = await Client.GetFromJsonAsync<CourseGradebookDto>($"/api/Gradebook/course/{courseId}");
        Assert.NotNull(gradebook);
        Assert.Single(gradebook!.Students);
        Assert.Equal(100, gradebook.Students[0].CurrentPercent);
        Assert.Equal(40, gradebook.Students[0].FinalPercent);

        var export = await Client.GetAsync($"/api/Gradebook/course/{courseId}/export");
        Assert.Equal(HttpStatusCode.OK, export.StatusCode);
        Assert.Equal("text/csv", export.Content.Headers.ContentType?.MediaType);
        var csv = await export.Content.ReadAsStringAsync();
        Assert.Contains("Student Name", csv);
        Assert.Contains("Final %", csv);
        Assert.Contains("Opening check", csv);
    }

    [Fact]
    public async Task UpdatePolicy_WhenWeightsDoNotTotal100_ReturnsBadRequest()
    {
        var courseId = await CreateCourseWithTwoQuizzesAsync("gradebook_invalid_trainer@example.com");
        var response = await Client.PutAsJsonAsync($"/api/Gradebook/course/{courseId}/policy", new UpdateGradebookPolicyDto
        {
            Categories =
            [
                new UpdateGradeCategoryDto { Name = "Practice", WeightPercent = 50, Order = 1 },
                new UpdateGradeCategoryDto { Name = "Final", WeightPercent = 30, Order = 2 }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<string> CreateCourseWithTwoQuizzesAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "GRADEBOOK_" + Guid.NewGuid().ToString("N")[..8];
        var createResponse = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Gradebook " + courseId,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 100,
            MaxParticipants = 10,
            Duration = 10,
            Status = CourseStatus.PendingReview
        });
        createResponse.EnsureSuccessStatusCode();

        var firstQuiz = await Client.PutAsJsonAsync($"/api/Quiz/course/{courseId}", BuildQuiz("Opening check"));
        firstQuiz.EnsureSuccessStatusCode();
        var secondQuiz = await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}", BuildQuiz("Final check"));
        secondQuiz.EnsureSuccessStatusCode();

        return courseId;
    }

    private static UpsertCourseQuizDto BuildQuiz(string title) => new()
    {
        Title = title,
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
    };

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

    private sealed class MyCourseGradesResponse
    {
        public string CourseId { get; set; } = string.Empty;
        public string? CourseTitle { get; set; }
        public List<GradeCategoryDto> Categories { get; set; } = new();
        public List<GradeItemDto> Items { get; set; } = new();
        public StudentGradeSummaryDto Grade { get; set; } = new();
    }
}
