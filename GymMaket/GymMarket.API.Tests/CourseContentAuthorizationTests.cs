using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

// Locks in the payment gate and trainer-ownership rules for course content.
public class CourseContentAuthorizationTests : BaseIntegrationTests
{
    public CourseContentAuthorizationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    // Authenticates as a trainer, creates a course they own with one lecture, and returns the ids.
    private async Task<(string courseId, string lectureId)> CreateOwnedCourseWithLectureAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");

        var courseId = "COURSE_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            TrainerId = GetTokenClaim("trainerId"),
            Title = "Auth Test Course",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Duration = 4,
            MaxParticipants = 20
        });

        var lectureId = "LECTURE_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        await Client.PostAsJsonAsync("/api/Lecture", new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Auth Test Lecture",
            Order = 1,
            Duration = 60
        });

        return (courseId, lectureId);
    }

    private async Task SeedPaidPaymentAsync(string studentId, string courseId)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        context.Payments.Add(new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Paid,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetLectures_AsUnpaidStudent_ReturnsForbidden()
    {
        var (courseId, _) = await CreateOwnedCourseWithLectureAsync("trainer-unpaid@example.com");

        // Switch to a student who has not paid for the course.
        await AuthenticateAsync(email: "student-unpaid@example.com", role: "Student");
        var response = await Client.GetAsync($"/api/Lecture/course/{courseId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetLectures_AsPaidStudent_ReturnsOk()
    {
        var (courseId, _) = await CreateOwnedCourseWithLectureAsync("trainer-paid@example.com");

        await AuthenticateAsync(email: "student-paid@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var response = await Client.GetAsync($"/api/Lecture/course/{courseId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLecture_AsNonOwnerTrainer_ReturnsForbidden()
    {
        var (_, lectureId) = await CreateOwnedCourseWithLectureAsync("owner-trainer@example.com");

        // A different trainer must not be able to manage someone else's lecture.
        await AuthenticateAsync(email: "other-trainer@example.com", role: "Trainer");
        var response = await Client.DeleteAsync($"/api/Lecture/{lectureId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
