using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Announcements;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseAnnouncementsIntegrationTests : BaseIntegrationTests
{
    public CourseAnnouncementsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task PublishedAnnouncement_NotifiesPaidStudentsAndIsVisibleToThem()
    {
        var courseId = await CreateCourseAsync("announcement_trainer@example.com");

        await AuthenticateAsync(email: "announcement_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: "announcement_trainer@example.com", role: "Trainer");
        var create = await Client.PostAsJsonAsync($"/api/CourseAnnouncements/course/{courseId}", new UpsertCourseAnnouncementDto
        {
            Title = "Schedule update",
            Body = "The live review moves to Friday at 5 PM.",
            IsPinned = true,
            IsPublished = true
        });

        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var announcement = await create.Content.ReadFromJsonAsync<CourseAnnouncementDto>();
        Assert.NotNull(announcement);
        Assert.True(announcement!.IsPublished);
        Assert.True(announcement.IsPinned);
        Assert.NotNull(announcement.PublishedAt);

        await AuthenticateAsync(email: "announcement_student@example.com", role: "Student");
        var feed = await Client.GetFromJsonAsync<List<CourseAnnouncementDto>>($"/api/CourseAnnouncements/course/{courseId}");
        Assert.Single(feed!);
        Assert.Equal("Schedule update", feed![0].Title);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications?type=announcement");
        var notification = Assert.Single(notifications!);
        Assert.Equal("Schedule update", notification.Title);
        Assert.Equal($"/client/course-announcements/{courseId}", notification.Link);
    }

    [Fact]
    public async Task DraftAnnouncement_IsHiddenFromStudentsUntilPublished()
    {
        var courseId = await CreateCourseAsync("announcement_draft_trainer@example.com");

        await AuthenticateAsync(email: "announcement_draft_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: "announcement_draft_trainer@example.com", role: "Trainer");
        var create = await Client.PostAsJsonAsync($"/api/CourseAnnouncements/course/{courseId}", new UpsertCourseAnnouncementDto
        {
            Title = "Draft note",
            Body = "This should not be visible yet.",
            IsPublished = false
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);

        await AuthenticateAsync(email: "announcement_draft_student@example.com", role: "Student");
        var feed = await Client.GetFromJsonAsync<List<CourseAnnouncementDto>>($"/api/CourseAnnouncements/course/{courseId}");
        Assert.Empty(feed!);
    }

    [Fact]
    public async Task UnpaidStudent_CannotReadCourseAnnouncements()
    {
        var courseId = await CreateCourseAsync("announcement_unpaid_trainer@example.com");

        await AuthenticateAsync(email: "announcement_unpaid_student@example.com", role: "Student");
        var response = await Client.GetAsync($"/api/CourseAnnouncements/course/{courseId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "ANNOUNCE_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Announcements " + courseId,
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
