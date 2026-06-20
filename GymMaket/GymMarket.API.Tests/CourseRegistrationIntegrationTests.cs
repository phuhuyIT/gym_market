using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseRegistrationIntegrationTests : BaseIntegrationTests
{
    public CourseRegistrationIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterCourse_WithValidData_RegistersTheAuthenticatedStudent()
    {
        // Arrange — a trainer creates the course…
        await AuthenticateAsync(email: "reg_trainer@example.com", role: "Trainer");
        var courseCreate = new CourseCreateDTO
        {
            CourseId = "CRS_REG_001",
            Title = "Registration Test Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        };
        await Client.PostAsJsonAsync("/api/Course", courseCreate);

        // …and a student registers for it. The student id comes from the JWT,
        // not the request body.
        await AuthenticateAsync(email: "student_reg@example.com");
        var registerDto = new RegisterCourseDto
        {
            CourseId = "CRS_REG_001"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterCourseResponseDto>();
        Assert.Equal(GetTokenClaim("studentId"), body!.Registration!.StudentId);
    }

    [Fact]
    public async Task RegisterCourse_AsTrainer_ReturnsForbidden()
    {
        // Arrange — trainers have no studentId claim, so they cannot register.
        await AuthenticateAsync(email: "reg_trainer2@example.com", role: "Trainer");

        // Act
        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_REG_001"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourseRegistrations_ReturnsOnlyOwnRegistrations()
    {
        // Arrange — a trainer creates a course and student A registers for it.
        await AuthenticateAsync(email: "reg_trainer3@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_REG_002",
            Title = "Isolation Test Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_a@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_REG_002"
        });

        // Act — student B asks for their registrations (the id comes from the JWT).
        await AuthenticateAsync(email: "student_b@example.com");
        var coursesOfB = await Client.GetFromJsonAsync<List<GetCourseDto>>("/api/CourseRegistration/get-course-registrations");

        // Assert — student A's registration is not visible to student B.
        Assert.NotNull(coursesOfB);
        Assert.Empty(coursesOfB!);

        await AuthenticateAsync(email: "student_a@example.com");
        var coursesOfA = await Client.GetFromJsonAsync<List<GetCourseDto>>("/api/CourseRegistration/get-course-registrations");
        Assert.Single(coursesOfA!);
    }

    [Fact]
    public async Task GetPaymentInfo_AsRegisteredStudent_ReturnsAmountStatusAndReference()
    {
        // Arrange — a trainer creates a course and the student registers for it.
        await AuthenticateAsync(email: "pay_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_PAY_001",
            Title = "Payment Info Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_pay@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_PAY_001"
        });

        // Act
        var response = await Client.GetAsync("/api/CourseRegistration/payment-info/CRS_PAY_001");

        // Assert — amount, pending status, and a matchable transfer reference are returned.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var info = await response.Content.ReadFromJsonAsync<CoursePaymentInfoDto>();
        Assert.NotNull(info);
        Assert.Equal(50, info!.Amount);
        Assert.Equal(PaymentStatus.Pending, info.Status);
        Assert.StartsWith("GYM", info.Reference);
        // The trainer set no bank account in this test, so payment is not yet configured.
        Assert.False(info.BankConfigured);
    }

    [Fact]
    public async Task GetPaymentInfo_ForUnregisteredCourse_ReturnsNotFound()
    {
        // Arrange — a trainer creates a course the student never registers for.
        await AuthenticateAsync(email: "pay_trainer2@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_PAY_002",
            Title = "Unregistered Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        // Act — a student with no registration asks for its payment info.
        await AuthenticateAsync(email: "student_nopay@example.com");
        var response = await Client.GetAsync("/api/CourseRegistration/payment-info/CRS_PAY_002");

        // Assert — no registration means no payment info is exposed.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaymentInfo_WhenPriceChangesBeforePaying_ReflectsTheNewPrice()
    {
        // Arrange — trainer creates a course at 50 and the student registers, which
        // snapshots a pending payment of 50.
        await AuthenticateAsync(email: "reprice_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_REPRICE_001",
            Title = "Repricing Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_reprice@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_REPRICE_001"
        });

        // The trainer raises the price before the student pays.
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var course = await db.Courses.FirstAsync(c => c.CourseId == "CRS_REPRICE_001");
            course.Price = 80;
            await db.SaveChangesAsync();
        }

        // Act — the student opens the payment screen again.
        var response = await Client.GetAsync("/api/CourseRegistration/payment-info/CRS_REPRICE_001");

        // Assert — the still-pending payment shows the new price, not the stale snapshot.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var info = await response.Content.ReadFromJsonAsync<CoursePaymentInfoDto>();
        Assert.Equal(80, info!.Amount);
        Assert.Equal(PaymentStatus.Pending, info.Status);
    }

    [Fact]
    public async Task ConfirmPayment_AsRegisteredStudent_ReturnsPendingInfo()
    {
        // Arrange — a trainer creates a course and the student registers for it.
        await AuthenticateAsync(email: "confirm_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_CONFIRM_001",
            Title = "Confirm Payment Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_confirm@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_CONFIRM_001"
        });

        // Act — the student taps "I've paid".
        var response = await Client.PostAsync("/api/CourseRegistration/confirm-payment/CRS_CONFIRM_001", null);

        // Assert — it never marks the payment paid; the trainer still confirms.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var info = await response.Content.ReadFromJsonAsync<CoursePaymentInfoDto>();
        Assert.NotNull(info);
        Assert.Equal(PaymentStatus.Pending, info!.Status);
    }

    [Fact]
    public async Task ConfirmPayment_ForUnregisteredCourse_ReturnsNotFound()
    {
        // Arrange — a trainer creates a course the student never registers for.
        await AuthenticateAsync(email: "confirm_trainer2@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_CONFIRM_002",
            Title = "Unregistered Confirm Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        // Act — a student with no registration tries to confirm.
        await AuthenticateAsync(email: "student_noconfirm@example.com");
        var response = await Client.PostAsync("/api/CourseRegistration/confirm-payment/CRS_CONFIRM_002", null);

        // Assert — no registration, nothing to confirm.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private class RegisterCourseResponseDto
    {
        public string? Message { get; set; }
        public CourseRegistration? Registration { get; set; }
    }
}
