using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
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

    [Fact]
    public async Task RegisterCourse_WhenPreviousPaymentWasCanceled_ReopensRegistrationWithNewPayment()
    {
        await AuthenticateAsync(email: "retry_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_RETRY_001",
            Title = "Retry Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_retry@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_RETRY_001"
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_RETRY_001");
            registration.PaymentStatus = PaymentStatus.Canceled;
            registration.Status = PaymentStatus.Canceled;
            var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_RETRY_001");
            payment.PaymentStatus = PaymentStatus.Canceled;
            await db.SaveChangesAsync();
        }

        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_RETRY_001"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var registrations = await db.CourseRegistrations.Where(r => r.CourseId == "CRS_RETRY_001").ToListAsync();
            var payments = await db.Payments.Where(p => p.CourseId == "CRS_RETRY_001").ToListAsync();

            Assert.Single(registrations);
            Assert.Equal(PaymentStatus.NotStarted, registrations.Single().PaymentStatus);
            Assert.Equal(2, payments.Count);
            Assert.Contains(payments, p => p.PaymentStatus == PaymentStatus.Pending);
        }
    }

    [Fact]
    public async Task RegisterCourse_WhenCapacityIsFull_ReturnsConflictWithCode()
    {
        await AuthenticateAsync(email: "capacity_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_CAPACITY_001",
            Title = "Small Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            MaxParticipants = 1
        });

        await AuthenticateAsync(email: "student_capacity_a@example.com");
        var first = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_CAPACITY_001"
        });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        await AuthenticateAsync(email: "student_capacity_b@example.com");
        var second = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_CAPACITY_001"
        });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        var body = await second.Content.ReadFromJsonAsync<RegisterCourseResponseDto>();
        Assert.Equal(CourseRegistrationErrorCode.CourseFull, body!.Message);
    }

    [Fact]
    public async Task RegisterCourse_WhenCourseIsDraft_ReturnsConflictWithCode()
    {
        await AuthenticateAsync(email: "draft_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_DRAFT_001",
            Title = "Draft Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            Status = CourseStatus.Draft
        });

        await AuthenticateAsync(email: "student_draft@example.com");
        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_DRAFT_001"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterCourseResponseDto>();
        Assert.Equal(CourseRegistrationErrorCode.CourseNotPublished, body!.Message);
    }

    [Fact]
    public async Task ConfirmGatewayPayment_MarksExactMomoAttemptPaidAndCancelsOtherPendingPayments()
    {
        await AuthenticateAsync(email: "momo_success_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_MOMO_SUCCESS_001",
            Title = "Momo Success Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_momo_success@example.com");
        var studentId = GetTokenClaim("studentId")!;
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_MOMO_SUCCESS_001"
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            db.Payments.Add(new Payment
            {
                PaymentId = "MOMO_SUCCESS_ORDER",
                CourseId = "CRS_MOMO_SUCCESS_001",
                StudentId = studentId,
                PaymentAmount = 50,
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = PaymentType.Momo,
                Note = "MOMO-SUCCESS",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var payments = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            await payments.ConfirmGatewayPayment("MOMO_SUCCESS_ORDER", PaymentType.Momo);
        }

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var momoPayment = await db.Payments.SingleAsync(p => p.PaymentId == "MOMO_SUCCESS_ORDER");
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_MOMO_SUCCESS_001");
            var payments = await db.Payments.Where(p => p.CourseId == "CRS_MOMO_SUCCESS_001").ToListAsync();

            Assert.Equal(PaymentStatus.Paid, momoPayment.PaymentStatus);
            Assert.Equal(PaymentStatus.Paid, registration.PaymentStatus);
            Assert.Contains(payments, p => p.PaymentId != "MOMO_SUCCESS_ORDER" && p.PaymentStatus == PaymentStatus.Canceled);
        }
    }

    [Fact]
    public async Task CancelGatewayPayment_MarksMomoAttemptCanceledAndRegistrationRetryable()
    {
        await AuthenticateAsync(email: "momo_cancel_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_MOMO_CANCEL_001",
            Title = "Momo Cancel Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_momo_cancel@example.com");
        var studentId = GetTokenClaim("studentId")!;
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_MOMO_CANCEL_001"
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            db.Payments.Add(new Payment
            {
                PaymentId = "MOMO_CANCEL_ORDER",
                CourseId = "CRS_MOMO_CANCEL_001",
                StudentId = studentId,
                PaymentAmount = 50,
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = PaymentType.Momo,
                Note = "MOMO-CANCEL",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var payments = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            await payments.CancelGatewayPayment("MOMO_CANCEL_ORDER", "User canceled payment.");
        }

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var momoPayment = await db.Payments.SingleAsync(p => p.PaymentId == "MOMO_CANCEL_ORDER");
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_MOMO_CANCEL_001");
            var payments = await db.Payments.Where(p => p.CourseId == "CRS_MOMO_CANCEL_001").ToListAsync();

            Assert.Equal(PaymentStatus.Canceled, momoPayment.PaymentStatus);
            Assert.Equal("User canceled payment.", momoPayment.Note);
            Assert.Equal(PaymentStatus.Canceled, registration.PaymentStatus);
            Assert.All(payments, p => Assert.Equal(PaymentStatus.Canceled, p.PaymentStatus));
        }
    }

    private class RegisterCourseResponseDto
    {
        public string? Message { get; set; }
        public CourseRegistration? Registration { get; set; }
    }
}
