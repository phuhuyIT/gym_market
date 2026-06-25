using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
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

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_REG_001");
        var paymentEvent = await db.PaymentEvents.SingleAsync(e => e.PaymentId == payment.PaymentId);

        Assert.Equal(PaymentEventType.Created, paymentEvent.EventType);
        Assert.Equal(PaymentEventSource.Student, paymentEvent.Source);
        Assert.Equal(PaymentStatus.Pending, paymentEvent.NewStatus);

        var trainerUserId = await db.Courses
            .Where(c => c.CourseId == "CRS_REG_001")
            .Select(c => c.Trainer!.UserId)
            .SingleAsync();
        var notification = await db.Notifications.SingleAsync(n =>
            n.UserId == trainerUserId
            && n.Type == NotificationTypes.Payment
            && n.Link == $"/agency/payments?studentId={body.Registration.StudentId}");
        Assert.Equal("New payment pending", notification.Title);
        Assert.False(notification.IsRead);
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
    public async Task ConfirmPaymentByStudent_MovesBankTransferToTrainerReviewAndPreventsShortExpiry()
    {
        await AuthenticateAsync(email: "manual_review_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_MANUAL_REVIEW_001",
            Title = "Manual Review Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_manual_review@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_MANUAL_REVIEW_001"
        });

        var confirmResponse = await Client.PostAsync("/api/CourseRegistration/confirm-payment/CRS_MANUAL_REVIEW_001", null);

        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
        var info = await confirmResponse.Content.ReadFromJsonAsync<CoursePaymentInfoDto>();
        Assert.Equal(PaymentStatus.AwaitingConfirmation, info!.Status);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_MANUAL_REVIEW_001");
        var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_MANUAL_REVIEW_001");
        var submittedEvent = await db.PaymentEvents.SingleAsync(e =>
            e.PaymentId == payment.PaymentId
            && e.EventType == PaymentEventType.ManualSubmitted);

        Assert.Equal(PaymentStatus.Pending, submittedEvent.OldStatus);
        Assert.Equal(PaymentStatus.AwaitingConfirmation, submittedEvent.NewStatus);
        Assert.Equal(PaymentStatus.AwaitingConfirmation, payment.PaymentStatus);
        Assert.Equal(PaymentStatus.AwaitingConfirmation, registration.PaymentStatus);

        payment.UpdatedAt = DateTime.UtcNow.AddMinutes(-(Defaults.PaymentTimeoutMinutes + 30));
        registration.UpdatedAt = payment.UpdatedAt;
        await db.SaveChangesAsync();

        var expiry = scope.ServiceProvider.GetRequiredService<IRegistrationExpiryService>();
        var expiredCount = await expiry.ExpireStalePendingRegistrationsAsync("CRS_MANUAL_REVIEW_001", registration.StudentId);

        Assert.Equal(0, expiredCount);
        Assert.Equal(PaymentStatus.AwaitingConfirmation, payment.PaymentStatus);
        Assert.Equal(PaymentStatus.AwaitingConfirmation, registration.PaymentStatus);
    }

    [Fact]
    public async Task RegisterCourse_WithSelectedOptions_IncludesOptionsInPaymentAmount()
    {
        await AuthenticateAsync(email: "option_pay_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_OPTION_PAY_001",
            Title = "Option Payment Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            AdditionalPrice = 5
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            db.CourseOptions.Add(new CourseOption
            {
                OptionId = "CRS_OPTION_ADDON_001",
                CourseId = "CRS_OPTION_PAY_001",
                OptionName = "Coaching Call",
                Price = 20
            });
            await db.SaveChangesAsync();
        }

        await AuthenticateAsync(email: "student_option_pay@example.com");
        var registerResponse = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_OPTION_PAY_001",
            OptionIds = ["CRS_OPTION_ADDON_001"]
        });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var info = await Client.GetFromJsonAsync<CoursePaymentInfoDto>("/api/CourseRegistration/payment-info/CRS_OPTION_PAY_001");

        Assert.NotNull(info);
        Assert.Equal(75, info!.Amount);
        Assert.Equal(55, info.CourseAmount);
        Assert.Equal(20, info.OptionsAmount);
        Assert.Single(info.Options);
        Assert.Equal("CRS_OPTION_ADDON_001", info.Options[0].OptionId);
    }

    [Fact]
    public async Task RegisterCourse_WithOptionFromAnotherCourse_ReturnsConflict()
    {
        await AuthenticateAsync(email: "option_invalid_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_OPTION_VALID",
            Title = "Valid Option Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_OPTION_OTHER",
            Title = "Other Option Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            db.CourseOptions.Add(new CourseOption
            {
                OptionId = "CRS_OPTION_WRONG_COURSE",
                CourseId = "CRS_OPTION_OTHER",
                OptionName = "Wrong Course Option",
                Price = 20
            });
            await db.SaveChangesAsync();
        }

        await AuthenticateAsync(email: "student_invalid_option@example.com");
        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_OPTION_VALID",
            OptionIds = ["CRS_OPTION_WRONG_COURSE"]
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
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
    public async Task ConfirmPayment_AsRegisteredStudent_ReturnsAwaitingConfirmationInfo()
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
        Assert.Equal(PaymentStatus.AwaitingConfirmation, info!.Status);
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

            var trainerUserId = await db.Courses
                .Where(c => c.CourseId == "CRS_RETRY_001")
                .Select(c => c.Trainer!.UserId)
                .SingleAsync();
            var notification = await db.Notifications.SingleAsync(n =>
                n.UserId == trainerUserId
                && n.Type == NotificationTypes.Payment
                && n.Link!.StartsWith("/agency/payments?studentId="));
            Assert.Equal("Payment restarted", notification.Title);
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
    public async Task RegisterCourse_WhenPreviousSeatReservationExpired_ReleasesCapacity()
    {
        await AuthenticateAsync(email: "capacity_expired_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_CAPACITY_EXPIRED_001",
            Title = "Capacity Expiry Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            MaxParticipants = 1
        });

        await AuthenticateAsync(email: "student_capacity_expired_a@example.com");
        var first = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_CAPACITY_EXPIRED_001"
        });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var staleTime = DateTime.UtcNow.AddMinutes(-(Defaults.PaymentTimeoutMinutes + 1));
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_CAPACITY_EXPIRED_001");
            registration.UpdatedAt = staleTime;
            registration.CreatedAt = staleTime;

            var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_CAPACITY_EXPIRED_001");
            payment.PaymentStatus = PaymentStatus.Pending;
            payment.UpdatedAt = staleTime;
            payment.CreatedAt = staleTime;
            await db.SaveChangesAsync();
        }

        await AuthenticateAsync(email: "student_capacity_expired_b@example.com");
        var second = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_CAPACITY_EXPIRED_001"
        });

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var registrations = await db.CourseRegistrations
                .Where(r => r.CourseId == "CRS_CAPACITY_EXPIRED_001")
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
            var payments = await db.Payments.Where(p => p.CourseId == "CRS_CAPACITY_EXPIRED_001").ToListAsync();

            Assert.Equal(2, registrations.Count);
            Assert.Contains(registrations, r => r.PaymentStatus == PaymentStatus.Expired);
            Assert.Contains(registrations, r => r.PaymentStatus == PaymentStatus.NotStarted);
            Assert.Contains(payments, p => p.PaymentStatus == PaymentStatus.Expired);
            Assert.Contains(payments, p => p.PaymentStatus == PaymentStatus.Pending);
        }
    }

    [Fact]
    public async Task GetPaymentInfo_WhenRegistrationExpired_ReturnsExpiredStatus()
    {
        await AuthenticateAsync(email: "payment_expired_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "CRS_PAYMENT_EXPIRED_001",
            Title = "Payment Expiry Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        });

        await AuthenticateAsync(email: "student_payment_expired@example.com");
        await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", new RegisterCourseDto
        {
            CourseId = "CRS_PAYMENT_EXPIRED_001"
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var staleTime = DateTime.UtcNow.AddMinutes(-(Defaults.PaymentTimeoutMinutes + 1));
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_PAYMENT_EXPIRED_001");
            registration.UpdatedAt = staleTime;
            registration.CreatedAt = staleTime;

            var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_PAYMENT_EXPIRED_001");
            payment.PaymentStatus = PaymentStatus.Pending;
            payment.UpdatedAt = staleTime;
            payment.CreatedAt = staleTime;
            await db.SaveChangesAsync();
        }

        var response = await Client.GetAsync("/api/CourseRegistration/payment-info/CRS_PAYMENT_EXPIRED_001");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var info = await response.Content.ReadFromJsonAsync<CoursePaymentInfoDto>();
        Assert.Equal(PaymentStatus.Expired, info!.Status);

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var registration = await db.CourseRegistrations.SingleAsync(r => r.CourseId == "CRS_PAYMENT_EXPIRED_001");
            var payment = await db.Payments.SingleAsync(p => p.CourseId == "CRS_PAYMENT_EXPIRED_001");

            Assert.Equal(PaymentStatus.Expired, registration.PaymentStatus);
            Assert.Equal(PaymentStatus.Expired, payment.PaymentStatus);

            var studentUserId = await db.Students
                .Where(s => s.StudentId == registration.StudentId)
                .Select(s => s.UserId)
                .SingleAsync();
            var notification = await db.Notifications.SingleAsync(n =>
                n.UserId == studentUserId
                && n.Type == NotificationTypes.Payment
                && n.Link == "/client/course-payment/CRS_PAYMENT_EXPIRED_001");
            Assert.Equal("Payment expired", notification.Title);
        }
    }

    [Fact]
    public async Task RegistrationExpiryService_ExpiresStaleOpenRegistrationsAndPayments()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var expiryService = scope.ServiceProvider.GetRequiredService<IRegistrationExpiryService>();
        var staleTime = DateTime.UtcNow.AddMinutes(-(Defaults.PaymentTimeoutMinutes + 1));

        db.CourseRegistrations.Add(new CourseRegistration
        {
            RegistrationId = "REG_EXPIRY_SERVICE_001",
            CourseId = "CRS_EXPIRY_SERVICE_001",
            StudentId = "STU_EXPIRY_SERVICE_001",
            Status = PaymentStatus.PendingPayment,
            PaymentStatus = PaymentStatus.NotStarted,
            CreatedAt = staleTime,
            UpdatedAt = staleTime
        });
        db.Payments.Add(new Payment
        {
            PaymentId = "PAY_EXPIRY_SERVICE_001",
            CourseId = "CRS_EXPIRY_SERVICE_001",
            StudentId = "STU_EXPIRY_SERVICE_001",
            PaymentAmount = 50,
            PaymentStatus = PaymentStatus.Pending,
            PaymentType = PaymentType.BankTransfer,
            Note = "",
            CreatedAt = staleTime,
            UpdatedAt = staleTime
        });
        await db.SaveChangesAsync();

        var expiredCount = await expiryService.ExpireStalePendingRegistrationsAsync();

        Assert.Equal(1, expiredCount);

        var registration = await db.CourseRegistrations.SingleAsync(r => r.RegistrationId == "REG_EXPIRY_SERVICE_001");
        var payment = await db.Payments.SingleAsync(p => p.PaymentId == "PAY_EXPIRY_SERVICE_001");

        Assert.Equal(PaymentStatus.Expired, registration.Status);
        Assert.Equal(PaymentStatus.Expired, registration.PaymentStatus);
        Assert.Equal(PaymentStatus.Expired, payment.PaymentStatus);
        Assert.Equal("Expired because payment was not completed in time.", payment.Note);

        var paymentEvent = await db.PaymentEvents.SingleAsync(e => e.PaymentId == "PAY_EXPIRY_SERVICE_001");
        Assert.Equal(PaymentEventType.Expired, paymentEvent.EventType);
        Assert.Equal(PaymentStatus.Pending, paymentEvent.OldStatus);
        Assert.Equal(PaymentStatus.Expired, paymentEvent.NewStatus);
        Assert.Equal(PaymentEventSource.System, paymentEvent.Source);
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

            var momoEvent = await db.PaymentEvents.SingleAsync(e =>
                e.PaymentId == "MOMO_SUCCESS_ORDER" && e.EventType == PaymentEventType.Paid);
            Assert.Equal(PaymentStatus.Pending, momoEvent.OldStatus);
            Assert.Equal(PaymentStatus.Paid, momoEvent.NewStatus);
            Assert.Equal(PaymentEventSource.Momo, momoEvent.Source);

            var replacedPayment = payments.Single(p => p.PaymentId != "MOMO_SUCCESS_ORDER");
            var replacedEvent = await db.PaymentEvents.SingleAsync(e =>
                e.PaymentId == replacedPayment.PaymentId && e.EventType == PaymentEventType.ReplacedBySuccessfulPayment);
            Assert.Equal(PaymentStatus.Canceled, replacedEvent.NewStatus);
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

            var momoEvent = await db.PaymentEvents.SingleAsync(e =>
                e.PaymentId == "MOMO_CANCEL_ORDER" && e.EventType == PaymentEventType.Canceled);
            Assert.Equal(PaymentStatus.Pending, momoEvent.OldStatus);
            Assert.Equal(PaymentStatus.Canceled, momoEvent.NewStatus);
            Assert.Equal(PaymentEventSource.Momo, momoEvent.Source);
        }
    }

    private class RegisterCourseResponseDto
    {
        public string? Message { get; set; }
        public CourseRegistration? Registration { get; set; }
    }
}
