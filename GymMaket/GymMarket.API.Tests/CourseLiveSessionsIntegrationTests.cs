using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseCalendar;
using GymMarket.API.DTOs.CourseLiveSessions;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseLiveSessionsIntegrationTests : BaseIntegrationTests
{
    public CourseLiveSessionsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task ScheduledLiveSession_NotifiesPaidStudentAndAppearsOnCalendar()
    {
        var trainerEmail = "live_trainer@example.com";
        var studentEmail = "live_student@example.com";
        var courseId = await CreateCourseAsync(trainerEmail);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var startsAt = DateTime.UtcNow.AddDays(5);
        var create = await Client.PostAsJsonAsync($"/api/CourseLiveSessions/course/{courseId}", new UpsertCourseLiveSessionDto
        {
            Title = "Form check-in",
            Description = "Review technique and questions.",
            StartsAt = startsAt,
            EndsAt = startsAt.AddHours(1),
            MeetingUrl = "https://meet.example.com/form-check-in",
            Status = CourseLiveSessionStatus.Scheduled,
            AttendanceRequired = true
        });

        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<CourseLiveSessionDto>();
        Assert.NotNull(created);
        Assert.Equal(CourseLiveSessionStatus.Scheduled, created!.Status);
        Assert.NotNull(created.PublishedAt);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        var sessions = await Client.GetFromJsonAsync<List<CourseLiveSessionDto>>($"/api/CourseLiveSessions/course/{courseId}");
        var session = Assert.Single(sessions!);
        Assert.Equal("Form check-in", session.Title);
        Assert.Equal("https://meet.example.com/form-check-in", session.MeetingUrl);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications?type=live_session");
        var notification = Assert.Single(notifications!);
        Assert.Equal("Live session scheduled", notification.Title);
        Assert.Equal($"/client/course-live-sessions/{courseId}", notification.Link);

        var calendar = await Client.GetFromJsonAsync<List<CourseCalendarItemDto>>($"/api/CourseCalendar/course/{courseId}");
        Assert.Contains(calendar!, item => item.Type == "live_session" && item.ItemId == created.LiveSessionId);
    }

    [Fact]
    public async Task DraftLiveSession_IsHiddenFromStudentButVisibleToTrainer()
    {
        var trainerEmail = "live_draft_trainer@example.com";
        var studentEmail = "live_draft_student@example.com";
        var courseId = await CreateCourseAsync(trainerEmail);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var startsAt = DateTime.UtcNow.AddDays(8);
        var create = await Client.PostAsJsonAsync($"/api/CourseLiveSessions/course/{courseId}", new UpsertCourseLiveSessionDto
        {
            Title = "Private planning room",
            StartsAt = startsAt,
            EndsAt = startsAt.AddHours(1),
            Status = CourseLiveSessionStatus.Draft
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);

        var manageSessions = await Client.GetFromJsonAsync<List<CourseLiveSessionDto>>($"/api/CourseLiveSessions/course/{courseId}/manage");
        Assert.Single(manageSessions!);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        var learnerSessions = await Client.GetFromJsonAsync<List<CourseLiveSessionDto>>($"/api/CourseLiveSessions/course/{courseId}");
        Assert.Empty(learnerSessions!);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "LIVE_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Live sessions " + courseId,
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
