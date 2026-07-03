using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Discussions;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseDiscussionsIntegrationTests : BaseIntegrationTests
{
    public CourseDiscussionsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task DiscussionQuestion_CanBeAnsweredAcceptedAndModerated()
    {
        var courseId = await CreateCourseAsync("discussion_trainer@example.com");

        await AuthenticateAsync(email: "discussion_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        var create = await Client.PostAsJsonAsync($"/api/CourseDiscussions/course/{courseId}/questions", new CreateDiscussionQuestionDto
        {
            Title = "How should I scale the workout?",
            Body = "I cannot finish the final circuit yet. What should I change?"
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var question = await create.Content.ReadFromJsonAsync<DiscussionQuestionDto>();
        Assert.NotNull(question);
        Assert.Equal(DiscussionQuestionStatus.Open, question!.Status);

        await AuthenticateAsync(email: "discussion_trainer@example.com", role: "Trainer");
        var trainerQuestions = await Client.GetFromJsonAsync<List<DiscussionQuestionDto>>($"/api/CourseDiscussions/course/{courseId}");
        Assert.Single(trainerQuestions!);

        var reply = await Client.PostAsJsonAsync($"/api/CourseDiscussions/questions/{question.QuestionId}/answers", new CreateDiscussionAnswerDto
        {
            Body = "Reduce the last circuit to two rounds and keep the rest periods."
        });
        Assert.Equal(HttpStatusCode.OK, reply.StatusCode);
        var answer = await reply.Content.ReadFromJsonAsync<DiscussionAnswerDto>();
        Assert.NotNull(answer);
        Assert.Equal(DiscussionAuthorRole.Trainer, answer!.AuthorRole);

        var pin = await Client.PutAsJsonAsync($"/api/CourseDiscussions/questions/{question.QuestionId}/moderation", new ModerateDiscussionQuestionDto
        {
            IsPinned = true
        });
        Assert.Equal(HttpStatusCode.OK, pin.StatusCode);
        var pinned = await pin.Content.ReadFromJsonAsync<DiscussionQuestionDto>();
        Assert.True(pinned!.IsPinned);

        await AuthenticateAsync(email: "discussion_student@example.com", role: "Student");
        var accept = await Client.PutAsJsonAsync($"/api/CourseDiscussions/questions/{question.QuestionId}/accept/{answer.AnswerId}", new { });
        Assert.Equal(HttpStatusCode.OK, accept.StatusCode);
        var accepted = await accept.Content.ReadFromJsonAsync<DiscussionQuestionDto>();
        Assert.NotNull(accepted);
        Assert.Equal(DiscussionQuestionStatus.Answered, accepted!.Status);
        Assert.Equal(answer.AnswerId, accepted.AcceptedAnswerId);
        Assert.Single(accepted.Answers, a => a.IsAccepted);
    }

    [Fact]
    public async Task CreateQuestion_AsUnpaidStudent_ReturnsForbidden()
    {
        var courseId = await CreateCourseAsync("discussion_unpaid_trainer@example.com");

        await AuthenticateAsync(email: "discussion_unpaid_student@example.com", role: "Student");
        var response = await Client.PostAsJsonAsync($"/api/CourseDiscussions/course/{courseId}/questions", new CreateDiscussionQuestionDto
        {
            Title = "Can I ask without access?",
            Body = "This should not be allowed."
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "DISCUSSION_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Discussion " + courseId,
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
