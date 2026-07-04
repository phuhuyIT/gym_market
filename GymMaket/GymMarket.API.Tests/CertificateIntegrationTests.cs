using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Certificate;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.DTOs.LectureProgress;
using GymMarket.API.DTOs.Quiz;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CertificateIntegrationTests : BaseIntegrationTests
{
    public CertificateIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task IssueCertificate_BeforeCompletion_ReturnsConflict()
    {
        var (courseId, _) = await CreateCourseWithLectureAndQuizAsync("cert_incomplete_trainer@example.com");

        await AuthenticateAsync(email: "cert_incomplete_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var response = await Client.PostAsync($"/api/Certificates/course/{courseId}/issue", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var status = await response.Content.ReadFromJsonAsync<CourseCompletionStatusDto>();
        Assert.NotNull(status);
        Assert.False(status!.IsCompleted);
    }

    [Fact]
    public async Task IssueCertificate_AfterLectureAndQuizCompletion_ReturnsCertificate()
    {
        var (courseId, lectureId) = await CreateCourseWithLectureAndQuizAsync("cert_complete_trainer@example.com");

        await AuthenticateAsync(email: "cert_complete_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);
        await CompleteLectureAsync(lectureId);
        await SubmitPassingQuizAsync(courseId);

        var response = await Client.PostAsync($"/api/Certificates/course/{courseId}/issue", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var certificate = await response.Content.ReadFromJsonAsync<CourseCertificateDto>();
        Assert.NotNull(certificate);
        Assert.Equal(courseId, certificate!.CourseId);
        Assert.False(string.IsNullOrWhiteSpace(certificate.VerificationCode));
    }

    [Fact]
    public async Task VerifyCertificate_WithIssuedCode_ReturnsCertificate()
    {
        var (courseId, lectureId) = await CreateCourseWithLectureAndQuizAsync("cert_verify_trainer@example.com");

        await AuthenticateAsync(email: "cert_verify_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);
        await CompleteLectureAsync(lectureId);
        await SubmitPassingQuizAsync(courseId);
        var issued = await Client.PostAsync($"/api/Certificates/course/{courseId}/issue", null);
        var certificate = await issued.Content.ReadFromJsonAsync<CourseCertificateDto>();

        Client.DefaultRequestHeaders.Authorization = null;
        var verified = await Client.GetFromJsonAsync<CourseCertificateDto>($"/api/Certificates/verify/{certificate!.VerificationCode}");

        Assert.NotNull(verified);
        Assert.Equal(certificate.CertificateId, verified!.CertificateId);
    }

    [Fact]
    public async Task UpdateCertificateSettings_AsTrainer_ReturnsSavedSettings()
    {
        var (courseId, _) = await CreateCourseWithLectureAndQuizAsync("cert_settings_trainer@example.com");

        var response = await Client.PutAsJsonAsync($"/api/Certificates/course/{courseId}/settings", new UpdateCourseCertificateSettingDto
        {
            IsEnabled = true,
            TemplateName = "Performance",
            CertificateTitle = "Strength Credential",
            BodyText = "Awarded for meeting the course credential policy.",
            AccentColor = "#16A34A",
            RequiredLecturePercent = 80,
            RequirePublishedQuizzes = false,
            RequirePublishedAssignments = true,
            RequiredAssignmentPercent = 70,
            MinimumFinalGradePercent = 75
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setting = await response.Content.ReadFromJsonAsync<CourseCertificateSettingDto>();
        Assert.NotNull(setting);
        Assert.Equal("Performance", setting!.TemplateName);
        Assert.Equal(80, setting.RequiredLecturePercent);
        Assert.False(setting.RequirePublishedQuizzes);
        Assert.True(setting.RequirePublishedAssignments);
        Assert.Equal(75, setting.MinimumFinalGradePercent);
    }

    [Fact]
    public async Task IssueCertificate_WithRelaxedCourseRules_ReturnsCertificate()
    {
        var (courseId, _) = await CreateCourseWithLectureAndQuizAsync("cert_relaxed_trainer@example.com");
        var settingsResponse = await Client.PutAsJsonAsync($"/api/Certificates/course/{courseId}/settings", new UpdateCourseCertificateSettingDto
        {
            IsEnabled = true,
            TemplateName = "Fast Track",
            CertificateTitle = "Course Participation",
            BodyText = "Awarded when participation requirements are met.",
            AccentColor = "#7C3AED",
            RequiredLecturePercent = 0,
            RequirePublishedQuizzes = false,
            RequirePublishedAssignments = false,
            RequiredAssignmentPercent = 0,
            MinimumFinalGradePercent = null
        });
        settingsResponse.EnsureSuccessStatusCode();

        await AuthenticateAsync(email: "cert_relaxed_student@example.com", role: "Student");
        await SeedPaidPaymentAsync(GetTokenClaim("studentId")!, courseId);

        var response = await Client.PostAsync($"/api/Certificates/course/{courseId}/issue", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var certificate = await response.Content.ReadFromJsonAsync<CourseCertificateDto>();
        Assert.NotNull(certificate);
        Assert.Equal("Fast Track", certificate!.Setting?.TemplateName);
    }

    private async Task<(string courseId, string lectureId)> CreateCourseWithLectureAndQuizAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "CERT_COURSE_" + Guid.NewGuid().ToString("N")[..8];
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

        var lectureId = "CERT_LECTURE_" + Guid.NewGuid().ToString("N")[..8];
        await Client.PostAsJsonAsync("/api/Lecture", new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Certificate Lecture",
            Order = 1,
            Duration = 30
        });

        var quizResponse = await Client.PutAsJsonAsync($"/api/Quiz/course/{courseId}", new UpsertCourseQuizDto
        {
            Title = "Certificate Check",
            PassingScorePercent = 70,
            IsPublished = true,
            Questions =
            [
                new UpsertQuizQuestionDto
                {
                    Prompt = "What should you do before training?",
                    Order = 1,
                    Points = 1,
                    Options =
                    [
                        new UpsertQuizOptionDto { Text = "Warm up", IsCorrect = true },
                        new UpsertQuizOptionDto { Text = "Skip hydration", IsCorrect = false }
                    ]
                }
            ]
        });
        quizResponse.EnsureSuccessStatusCode();

        return (courseId, lectureId);
    }

    private async Task CompleteLectureAsync(string lectureId)
    {
        var response = await Client.PutAsJsonAsync($"/api/LectureProgress/lecture/{lectureId}", new UpdateLectureProgressDto
        {
            IsCompleted = true
        });
        response.EnsureSuccessStatusCode();
    }

    private async Task SubmitPassingQuizAsync(string courseId)
    {
        var quiz = await Client.GetFromJsonAsync<CourseQuizDto>($"/api/Quiz/course/{courseId}");
        var response = await Client.PostAsJsonAsync($"/api/Quiz/course/{courseId}/submit", new SubmitQuizAttemptDto
        {
            Answers =
            [
                new SubmitQuizAnswerDto
                {
                    QuestionId = quiz!.Questions[0].QuestionId,
                    SelectedOptionId = quiz.Questions[0].Options[0].OptionId
                }
            ]
        });
        response.EnsureSuccessStatusCode();
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
