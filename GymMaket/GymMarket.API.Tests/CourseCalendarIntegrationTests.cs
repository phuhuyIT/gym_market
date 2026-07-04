using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseCalendar;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseCalendarIntegrationTests : BaseIntegrationTests
{
    public CourseCalendarIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task MyCalendar_AsPaidStudent_ReturnsPublishedCourseItems()
    {
        var trainerEmail = "calendar_trainer@example.com";
        var studentEmail = "calendar_student@example.com";
        var courseId = await CreateCourseAsync(trainerEmail);
        var dueAt = DateTime.UtcNow.AddDays(3);
        await SeedAssignmentAsync(courseId, "Calendar assignment", dueAt, AssignmentStatus.Published);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var items = await Client.GetFromJsonAsync<List<CourseCalendarItemDto>>(
            $"/api/CourseCalendar/me?from={Uri.EscapeDataString(DateTime.UtcNow.AddDays(-1).ToString("O"))}&to={Uri.EscapeDataString(DateTime.UtcNow.AddDays(10).ToString("O"))}");

        Assert.Contains(items!, item =>
            item.CourseId == courseId
            && item.Type == "assignment"
            && item.Title == "Calendar assignment"
            && item.CourseTitle != null);
    }

    [Fact]
    public async Task TrainerCalendar_AsOwner_ReturnsDraftAndPublishedItems()
    {
        var trainerEmail = "calendar_owner@example.com";
        var courseId = await CreateCourseAsync(trainerEmail);
        await SeedAssignmentAsync(courseId, "Draft planning task", DateTime.UtcNow.AddDays(5), AssignmentStatus.Draft);

        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var items = await Client.GetFromJsonAsync<List<CourseCalendarItemDto>>(
            $"/api/CourseCalendar/trainer?from={Uri.EscapeDataString(DateTime.UtcNow.AddDays(-1).ToString("O"))}&to={Uri.EscapeDataString(DateTime.UtcNow.AddDays(10).ToString("O"))}");

        Assert.Contains(items!, item =>
            item.CourseId == courseId
            && item.Type == "assignment"
            && item.Status == AssignmentStatus.Draft);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "CAL_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Calendar " + courseId,
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

    private async Task SeedAssignmentAsync(string courseId, string title, DateTime dueAt, string status)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        db.CourseAssignments.Add(new CourseAssignment
        {
            AssignmentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = title,
            Instructions = "Calendar test instructions",
            DueAt = dueAt,
            Status = status,
            PointsPossible = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
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
