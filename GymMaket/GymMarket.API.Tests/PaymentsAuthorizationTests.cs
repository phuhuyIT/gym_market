using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

// Locks in the ownership rules for payment management and course updates:
// a trainer may only act on courses (and their payments) that they own.
public class PaymentsAuthorizationTests : BaseIntegrationTests
{
    public PaymentsAuthorizationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetPaymentsOfCourse_OtherTrainersCourse_ReturnsForbidden()
    {
        // Arrange — trainer A owns the course.
        var courseId = await CreateOwnedCourseAsync("payments_owner@example.com");

        // Act — trainer B asks for its payments.
        await AuthenticateAsync(email: "payments_other@example.com", role: "Trainer");
        var response = await Client.GetAsync($"/api/Payments/get-payments-of-course/{courseId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // The owner can still read them.
        await AuthenticateAsync(email: "payments_owner@example.com", role: "Trainer");
        var ownerResponse = await Client.GetAsync($"/api/Payments/get-payments-of-course/{courseId}");
        Assert.Equal(HttpStatusCode.OK, ownerResponse.StatusCode);
    }

    [Fact]
    public async Task OkPayment_OtherTrainersCourse_ReturnsForbidden()
    {
        // Arrange — a pending payment exists on trainer A's course.
        var courseId = await CreateOwnedCourseAsync("okpay_owner@example.com");
        var paymentId = await SeedPendingPaymentAsync(courseId);

        // Act — trainer B tries to approve it.
        await AuthenticateAsync(email: "okpay_other@example.com", role: "Trainer");
        var response = await Client.PostAsync($"/api/Payments/ok-payment/{paymentId}", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CancelPayment_OtherTrainersCourse_ReturnsForbidden()
    {
        // Arrange
        var courseId = await CreateOwnedCourseAsync("cancelpay_owner@example.com");
        var paymentId = await SeedPendingPaymentAsync(courseId);

        // Act
        await AuthenticateAsync(email: "cancelpay_other@example.com", role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/Payments/cancel-payment", new CancelPayment
        {
            PaymentId = paymentId,
            Note = "trying to cancel someone else's payment"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPaymentEvents_OwnerTrainer_ReturnsSortedHistory()
    {
        var courseId = await CreateOwnedCourseAsync("events_owner@example.com");
        var paymentId = await SeedPendingPaymentAsync(courseId);
        await SeedPaymentEventAsync(
            paymentId,
            PaymentEventType.Paid,
            PaymentStatus.Pending,
            PaymentStatus.Paid,
            PaymentEventSource.Trainer,
            DateTime.UtcNow.AddMinutes(1));
        await SeedPaymentEventAsync(
            paymentId,
            PaymentEventType.Created,
            null,
            PaymentStatus.Pending,
            PaymentEventSource.Student,
            DateTime.UtcNow);

        await AuthenticateAsync(email: "events_owner@example.com", role: "Trainer");
        var response = await Client.GetAsync($"/api/Payments/{paymentId}/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var events = await response.Content.ReadFromJsonAsync<List<PaymentEventDto>>();
        Assert.NotNull(events);
        Assert.Equal([PaymentEventType.Created, PaymentEventType.Paid], events!.Select(e => e.EventType).ToArray());
        Assert.All(events, e => Assert.Equal(paymentId, e.PaymentId));
    }

    [Fact]
    public async Task GetPaymentEvents_OtherTrainersCourse_ReturnsForbidden()
    {
        var courseId = await CreateOwnedCourseAsync("events_forbid_owner@example.com");
        var paymentId = await SeedPendingPaymentAsync(courseId);
        await SeedPaymentEventAsync(
            paymentId,
            PaymentEventType.Created,
            null,
            PaymentStatus.Pending,
            PaymentEventSource.Student,
            DateTime.UtcNow);

        await AuthenticateAsync(email: "events_forbid_other@example.com", role: "Trainer");
        var response = await Client.GetAsync($"/api/Payments/{paymentId}/events");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPaymentEvents_Admin_ReturnsHistoryForAnyCourse()
    {
        var courseId = await CreateOwnedCourseAsync("events_admin_owner@example.com");
        var paymentId = await SeedPendingPaymentAsync(courseId);
        await SeedPaymentEventAsync(
            paymentId,
            PaymentEventType.Created,
            null,
            PaymentStatus.Pending,
            PaymentEventSource.Student,
            DateTime.UtcNow);

        await AuthenticateAsAdminAsync("events_admin@example.com");
        var response = await Client.GetAsync($"/api/Payments/{paymentId}/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var events = await response.Content.ReadFromJsonAsync<List<PaymentEventDto>>();
        Assert.Single(events!);
    }

    [Fact]
    public async Task OkPayment_CanceledPayment_ReturnsConflictAndDoesNotApprove()
    {
        var courseId = await CreateOwnedCourseAsync("okpay_canceled_owner@example.com");
        var paymentId = await SeedPaymentAsync(courseId, PaymentStatus.Canceled);

        await AuthenticateAsync(email: "okpay_canceled_owner@example.com", role: "Trainer");
        var response = await Client.PostAsync($"/api/Payments/ok-payment/{paymentId}", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PaymentActionResponse>();
        Assert.Equal(PaymentErrorCode.PaymentAlreadyFinalized, body!.ErrorCode);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.FindAsync(paymentId);
        Assert.Equal(PaymentStatus.Canceled, payment!.PaymentStatus);
    }

    [Fact]
    public async Task OkPayment_ObsoletePendingPayment_ReturnsConflictAndDoesNotChangeRegistration()
    {
        var courseId = await CreateOwnedCourseAsync("okpay_obsolete_owner@example.com");
        var studentId = "STU_OBSOLETE_PAYMENT";
        var obsoletePaymentId = await SeedPaymentAsync(courseId, PaymentStatus.Pending, studentId);
        await SeedPaymentAsync(courseId, PaymentStatus.Paid, studentId);

        await AuthenticateAsync(email: "okpay_obsolete_owner@example.com", role: "Trainer");
        var response = await Client.PostAsync($"/api/Payments/ok-payment/{obsoletePaymentId}", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PaymentActionResponse>();
        Assert.Equal(PaymentErrorCode.PaymentObsolete, body!.ErrorCode);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.FindAsync(obsoletePaymentId);
        Assert.Equal(PaymentStatus.Pending, payment!.PaymentStatus);
    }

    [Fact]
    public async Task OkPayment_OwnerApprovesPendingPayment_WritesAuditEvent()
    {
        var courseId = await CreateOwnedCourseAsync("okpay_audit_owner@example.com");
        var paymentId = await SeedPaymentAsync(courseId, PaymentStatus.Pending);

        await AuthenticateAsync(email: "okpay_audit_owner@example.com", role: "Trainer");
        var response = await Client.PostAsync($"/api/Payments/ok-payment/{paymentId}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.FindAsync(paymentId);
        var paymentEvent = await context.PaymentEvents.SingleAsync(e => e.PaymentId == paymentId);

        Assert.Equal(PaymentStatus.Paid, payment!.PaymentStatus);
        Assert.Equal(PaymentEventType.ManualApproved, paymentEvent.EventType);
        Assert.Equal(PaymentStatus.Pending, paymentEvent.OldStatus);
        Assert.Equal(PaymentStatus.Paid, paymentEvent.NewStatus);
        Assert.Equal(PaymentEventSource.Trainer, paymentEvent.Source);
    }

    [Fact]
    public async Task CancelPayment_PaidPayment_ReturnsConflictAndKeepsPaid()
    {
        var courseId = await CreateOwnedCourseAsync("cancelpay_paid_owner@example.com");
        var paymentId = await SeedPaymentAsync(courseId, PaymentStatus.Paid);

        await AuthenticateAsync(email: "cancelpay_paid_owner@example.com", role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/Payments/cancel-payment", new CancelPayment
        {
            PaymentId = paymentId,
            Note = "trying to cancel a paid payment"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PaymentActionResponse>();
        Assert.Equal(PaymentErrorCode.PaymentAlreadyFinalized, body!.ErrorCode);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.FindAsync(paymentId);
        Assert.Equal(PaymentStatus.Paid, payment!.PaymentStatus);
    }

    [Fact]
    public async Task UpdateCourse_OtherTrainersCourse_ReturnsForbidden()
    {
        // Arrange — trainer A owns the course.
        var courseId = await CreateOwnedCourseAsync("update_owner@example.com");

        // Act — trainer B tries to update it (multipart, like the Angular client).
        await AuthenticateAsync(email: "update_other@example.com", role: "Trainer");
        using var form = new MultipartFormDataContent
        {
            { new StringContent(courseId), "CourseId" },
            { new StringContent("Hijacked Title"), "Title" }
        };
        var response = await Client.PutAsync("/api/Course/update-course", form);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCourse_OtherTrainersCourse_ReturnsForbidden()
    {
        // Arrange
        var courseId = await CreateOwnedCourseAsync("delete_owner@example.com");

        // Act
        await AuthenticateAsync(email: "delete_other@example.com", role: "Trainer");
        var response = await Client.DeleteAsync($"/api/Course/{courseId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // Authenticates as the given trainer and creates a course they own. The
    // backend stamps the owner from the trainerId JWT claim.
    private async Task<string> CreateOwnedCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");

        var courseId = "COURSE_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Payments Auth Test Course",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Duration = 4,
            MaxParticipants = 20
        });
        Assert.True(response.IsSuccessStatusCode);

        return courseId;
    }

    private async Task<string> SeedPendingPaymentAsync(string courseId)
    {
        return await SeedPaymentAsync(courseId, PaymentStatus.Pending);
    }

    private async Task<string> SeedPaymentAsync(string courseId, string status, string studentId = "STU_PAYMENT_AUTH")
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            StudentId = studentId,
            PaymentAmount = 100,
            PaymentStatus = status,
            PaymentType = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PaymentDate = DateTime.UtcNow
        };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();
        return payment.PaymentId;
    }

    private async Task SeedPaymentEventAsync(
        string paymentId,
        string eventType,
        string? oldStatus,
        string? newStatus,
        string source,
        DateTime createdAt)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        context.PaymentEvents.Add(new PaymentEvent
        {
            PaymentEventId = Guid.NewGuid().ToString(),
            PaymentId = paymentId,
            EventType = eventType,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Source = source,
            Message = $"{eventType} event",
            CreatedAt = createdAt
        });
        await context.SaveChangesAsync();
    }

    private class PaymentActionResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
    }
}
