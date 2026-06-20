using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
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
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            StudentId = "STU_PAYMENT_AUTH",
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Pending,
            PaymentType = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PaymentDate = DateTime.UtcNow
        };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();
        return payment.PaymentId;
    }
}
