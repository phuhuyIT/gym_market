using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.DTOs.LectureProgress;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class LectureProgressIntegrationTests : BaseIntegrationTests
{
    public LectureProgressIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateProgress_AsUnpaidStudent_ReturnsForbidden()
    {
        var (_, lectureId) = await CreateCourseWithLectureAsync("progress_unpaid_trainer@example.com");

        await AuthenticateAsync(email: "progress_unpaid_student@example.com", role: "Student");
        var response = await Client.PutAsJsonAsync($"/api/LectureProgress/lecture/{lectureId}", new UpdateLectureProgressDto
        {
            IsCompleted = true
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAndGetProgress_AsPaidStudent_PersistsCompletedLecture()
    {
        var (courseId, lectureId) = await CreateCourseWithLectureAsync("progress_paid_trainer@example.com");

        await AuthenticateAsync(email: "progress_paid_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var updateResponse = await Client.PutAsJsonAsync($"/api/LectureProgress/lecture/{lectureId}", new UpdateLectureProgressDto
        {
            IsCompleted = true
        });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var progress = await Client.GetFromJsonAsync<CourseProgressDto>($"/api/LectureProgress/course/{courseId}");

        Assert.NotNull(progress);
        Assert.Equal(1, progress!.TotalLectures);
        Assert.Equal(1, progress.CompletedLectures);
        Assert.Equal(100, progress.ProgressPercent);
        Assert.Contains(lectureId, progress.CompletedLectureIds);
    }

    private async Task<(string courseId, string lectureId)> CreateCourseWithLectureAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "PROGRESS_COURSE_" + Guid.NewGuid().ToString("N")[..8];
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = courseId,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 100,
            MaxParticipants = 10,
            Duration = 10,
            Status = CourseStatus.PendingReview
        });

        var lectureId = "PROGRESS_LECTURE_" + Guid.NewGuid().ToString("N")[..8];
        await Client.PostAsJsonAsync("/api/Lecture", new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Progress Lecture",
            Order = 1,
            Duration = 30
        });

        return (courseId, lectureId);
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
