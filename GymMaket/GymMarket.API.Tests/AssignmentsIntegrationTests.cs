using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Assignments;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Gradebook;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class AssignmentsIntegrationTests : BaseIntegrationTests
{
    public AssignmentsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Assignment_CanBeSubmittedGradedAndIncludedInGradebook()
    {
        var courseId = await CreateCourseAsync("assignment_trainer@example.com");
        var created = await Client.PostAsJsonAsync($"/api/Assignments/course/{courseId}", new UpsertCourseAssignmentDto
        {
            Title = "Training reflection",
            Instructions = "Explain how you adjusted your plan.",
            PointsPossible = 20,
            SubmissionType = AssignmentSubmissionType.Text,
            Status = AssignmentStatus.Published
        });
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var assignment = await created.Content.ReadFromJsonAsync<CourseAssignmentDto>();
        Assert.NotNull(assignment);

        await AuthenticateAsync(email: "assignment_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        var studentAssignments = await Client.GetFromJsonAsync<List<CourseAssignmentDto>>($"/api/Assignments/course/{courseId}");
        Assert.Single(studentAssignments!);
        Assert.Null(studentAssignments![0].MySubmission);

        var submit = await Client.PostAsJsonAsync($"/api/Assignments/{assignment!.AssignmentId}/submit", new SubmitAssignmentDto
        {
            TextResponse = "I reduced intensity and added recovery work."
        });
        Assert.Equal(HttpStatusCode.OK, submit.StatusCode);
        var submission = await submit.Content.ReadFromJsonAsync<AssignmentSubmissionDto>();
        Assert.NotNull(submission);
        Assert.Equal(AssignmentSubmissionStatus.Submitted, submission!.Status);

        await AuthenticateAsync(email: "assignment_trainer@example.com", role: "Trainer");
        var grade = await Client.PutAsJsonAsync($"/api/Assignments/submissions/{submission.SubmissionId}/grade", new GradeAssignmentSubmissionDto
        {
            Score = 17,
            Feedback = "Good reasoning."
        });
        Assert.Equal(HttpStatusCode.OK, grade.StatusCode);
        var graded = await grade.Content.ReadFromJsonAsync<AssignmentSubmissionDto>();
        Assert.NotNull(graded);
        Assert.Equal(85, graded!.ScorePercent);

        var gradebook = await Client.GetFromJsonAsync<CourseGradebookDto>($"/api/Gradebook/course/{courseId}");
        Assert.NotNull(gradebook);
        var item = Assert.Single(gradebook!.Items);
        Assert.Equal("Assignment", item.ItemType);
        Assert.Equal(assignment.AssignmentId, item.ItemId);
        var student = Assert.Single(gradebook.Students);
        Assert.Equal(85, student.FinalPercent);
        Assert.Equal("B", student.LetterGrade);

        var export = await Client.GetAsync($"/api/Gradebook/course/{courseId}/export");
        Assert.Equal(HttpStatusCode.OK, export.StatusCode);
        var csv = await export.Content.ReadAsStringAsync();
        Assert.Contains("Training reflection", csv);
        Assert.Contains("85", csv);
    }

    [Fact]
    public async Task SubmitAssignment_AsUnpaidStudent_ReturnsForbidden()
    {
        var courseId = await CreateCourseAsync("assignment_unpaid_trainer@example.com");
        var created = await Client.PostAsJsonAsync($"/api/Assignments/course/{courseId}", new UpsertCourseAssignmentDto
        {
            Title = "Unpaid work",
            PointsPossible = 10,
            Status = AssignmentStatus.Published
        });
        var assignment = await created.Content.ReadFromJsonAsync<CourseAssignmentDto>();

        await AuthenticateAsync(email: "assignment_unpaid_student@example.com", role: "Student");
        var response = await Client.PostAsJsonAsync($"/api/Assignments/{assignment!.AssignmentId}/submit", new SubmitAssignmentDto
        {
            TextResponse = "Cannot submit without payment."
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "ASSIGNMENT_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Assignment " + courseId,
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
