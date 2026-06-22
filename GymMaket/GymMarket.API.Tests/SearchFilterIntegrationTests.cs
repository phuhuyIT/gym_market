using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Student;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class SearchFilterIntegrationTests : BaseIntegrationTests
{
    public SearchFilterIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SearchPayments_WithTypeStatusAndDateRange_ReturnsMatchingPayments()
    {
        await AuthenticateAsAdminAsync();
        await SeedSearchDataAsync();

        var from = Uri.EscapeDataString("2026-06-01");
        var to = Uri.EscapeDataString("2026-06-30");
        var result = await Client.GetFromJsonAsync<PagedResult<GetPaymentDto>>(
            $"/api/Payments/search?status=Paid&paymentType=MOMO&fromDate={from}&toDate={to}&pageIndex=1&pageSize=10");

        Assert.NotNull(result);
        Assert.Equal(1, result!.TotalCount);
        Assert.Equal("PAY_MATCH", result.Items.Single().PaymentId);
        Assert.Equal(PaymentStatus.Paid, result.Items.Single().PaymentStatus);
    }

    [Fact]
    public async Task SearchStudents_WithHealthAccountAndPaymentFilters_ReturnsMatchingStudents()
    {
        await AuthenticateAsAdminAsync();
        await SeedSearchDataAsync();

        var result = await Client.GetFromJsonAsync<PagedResult<StudentSearchDto>>(
            "/api/Student/search?healthStatus=Healthy&status=Active&paymentStatus=Paid&pageIndex=1&pageSize=10");

        Assert.NotNull(result);
        Assert.Equal(1, result!.TotalCount);
        Assert.Equal("STU_MATCH", result.Items.Single().StudentId);
    }

    [Fact]
    public async Task SearchTrainers_WithRatingExperienceAndStatusFilters_ReturnsMatchingTrainers()
    {
        await AuthenticateAsAdminAsync();
        await SeedSearchDataAsync();

        var result = await Client.GetFromJsonAsync<PagedResult<TrainerSearchDto>>(
            "/api/Trainer/search?category=Strength&minRating=4.5&minExperience=8&status=Active&pageIndex=1&pageSize=10");

        Assert.NotNull(result);
        Assert.Equal(1, result!.TotalCount);
        Assert.Equal("TR_MATCH", result.Items.Single().TrainerId);
    }

    private async Task SeedSearchDataAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();

        var activeUser = new AppUser
        {
            Id = "USER_STU_MATCH",
            UserName = "student.match@example.com",
            NormalizedUserName = "STUDENT.MATCH@EXAMPLE.COM",
            Email = "student.match@example.com",
            NormalizedEmail = "STUDENT.MATCH@EXAMPLE.COM",
            FullName = "Student Match",
            Status = "Active"
        };
        var inactiveUser = new AppUser
        {
            Id = "USER_STU_OTHER",
            UserName = "student.other@example.com",
            NormalizedUserName = "STUDENT.OTHER@EXAMPLE.COM",
            Email = "student.other@example.com",
            NormalizedEmail = "STUDENT.OTHER@EXAMPLE.COM",
            FullName = "Student Other",
            Status = "Inactive"
        };
        var activeTrainerUser = new AppUser
        {
            Id = "USER_TR_MATCH",
            UserName = "trainer.match@example.com",
            NormalizedUserName = "TRAINER.MATCH@EXAMPLE.COM",
            Email = "trainer.match@example.com",
            NormalizedEmail = "TRAINER.MATCH@EXAMPLE.COM",
            FullName = "Trainer Match",
            Status = "Active"
        };
        var inactiveTrainerUser = new AppUser
        {
            Id = "USER_TR_OTHER",
            UserName = "trainer.other@example.com",
            NormalizedUserName = "TRAINER.OTHER@EXAMPLE.COM",
            Email = "trainer.other@example.com",
            NormalizedEmail = "TRAINER.OTHER@EXAMPLE.COM",
            FullName = "Trainer Other",
            Status = "Inactive"
        };

        context.Users.AddRange(activeUser, inactiveUser, activeTrainerUser, inactiveTrainerUser);

        context.Students.AddRange(
            new Student
            {
                StudentId = "STU_MATCH",
                UserId = activeUser.Id,
                Name = "Student Match",
                Email = activeUser.Email,
                HealthStatus = "Healthy",
                CreatedAt = new DateTime(2026, 6, 1)
            },
            new Student
            {
                StudentId = "STU_OTHER",
                UserId = inactiveUser.Id,
                Name = "Student Other",
                Email = inactiveUser.Email,
                HealthStatus = "Needs Attention",
                CreatedAt = new DateTime(2026, 6, 2)
            });

        context.Trainers.AddRange(
            new Trainer
            {
                TrainerId = "TR_MATCH",
                UserId = activeTrainerUser.Id,
                Name = "Strength Match",
                Email = activeTrainerUser.Email,
                Category = "Strength",
                Certification = "Strength Coach",
                Bio = "Strength programming",
                Description = "Barbell coaching",
                Experience = 10,
                Rating = 4.8m,
                CreatedAt = new DateTime(2026, 6, 1)
            },
            new Trainer
            {
                TrainerId = "TR_OTHER",
                UserId = inactiveTrainerUser.Id,
                Name = "Strength Other",
                Email = inactiveTrainerUser.Email,
                Category = "Strength",
                Certification = "Strength Coach",
                Bio = "Strength programming",
                Description = "Starter coaching",
                Experience = 4,
                Rating = 4.2m,
                CreatedAt = new DateTime(2026, 6, 2)
            });

        context.Courses.Add(new Course
        {
            CourseId = "COURSE_MATCH",
            TrainerId = "TR_MATCH",
            Title = "Strength Course",
            Category = "Strength",
            Type = "Online",
            Price = 100
        });

        context.Payments.AddRange(
            new Payment
            {
                PaymentId = "PAY_MATCH",
                CourseId = "COURSE_MATCH",
                StudentId = "STU_MATCH",
                PaymentAmount = 100,
                PaymentStatus = PaymentStatus.Completed,
                PaymentType = "MOMO",
                PaymentDate = new DateTime(2026, 6, 15),
                CreatedAt = new DateTime(2026, 6, 15)
            },
            new Payment
            {
                PaymentId = "PAY_OTHER_TYPE",
                CourseId = "COURSE_MATCH",
                StudentId = "STU_OTHER",
                PaymentAmount = 100,
                PaymentStatus = PaymentStatus.Paid,
                PaymentType = "Cash",
                PaymentDate = new DateTime(2026, 6, 15),
                CreatedAt = new DateTime(2026, 6, 15)
            },
            new Payment
            {
                PaymentId = "PAY_OTHER_DATE",
                CourseId = "COURSE_MATCH",
                StudentId = "STU_OTHER",
                PaymentAmount = 100,
                PaymentStatus = PaymentStatus.Paid,
                PaymentType = "MOMO",
                PaymentDate = new DateTime(2026, 5, 15),
                CreatedAt = new DateTime(2026, 5, 15)
            });

        context.CourseRegistrations.Add(new CourseRegistration
        {
            RegistrationId = "REG_MATCH",
            CourseId = "COURSE_MATCH",
            StudentId = "STU_MATCH",
            PaymentStatus = PaymentStatus.Paid,
            Status = PaymentStatus.Paid,
            CreatedAt = new DateTime(2026, 6, 15)
        });

        await context.SaveChangesAsync();
    }
}
