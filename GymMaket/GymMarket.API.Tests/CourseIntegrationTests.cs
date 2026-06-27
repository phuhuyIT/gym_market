using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseIntegrationTests : BaseIntegrationTests
{
    public CourseIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCourse_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync(role: "Trainer");
        var model = new CourseCreateDTO
        {
            CourseId = "COURSE_001",
            Title = "Test Course",
            Description = "A course for testing",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Duration = 4,
            MaxParticipants = 20
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Course", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_WithPendingTrainer_ReturnsForbiddenUntilApproved()
    {
        var email = "pending_course_trainer@example.com";
        var password = "Password123";
        var signupResponse = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Pending Course Trainer",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Trainer"
        });
        signupResponse.EnsureSuccessStatusCode();
        var signup = (await signupResponse.Content.ReadFromJsonAsync<SignupResponseDto>())!;
        await ConfirmUserEmailAsync(email);

        var loginResponse = await Client.PostAsJsonAsync("/api/Accounts/login", new LoginDto
        {
            Email = email,
            Password = password
        });
        var login = (await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>())!;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

        var pendingResponse = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_PENDING_TRAINER",
            Title = "Pending Trainer Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30)
        });
        var pendingBody = await pendingResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Forbidden, pendingResponse.StatusCode);
        Assert.Contains("TRAINER_NOT_APPROVED", pendingBody);

        await AuthenticateAsAdminAsync(email: "pending-course-admin@example.com");
        var approvalResponse = await Client.PutAsJsonAsync(
            $"/api/Admin/users/{signup.UserId}/trainer-approval",
            new UpdateTrainerApprovalDto { Status = TrainerApprovalStatus.Approved });
        Assert.Equal(HttpStatusCode.OK, approvalResponse.StatusCode);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
        var approvedResponse = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_APPROVED_TRAINER",
            Title = "Approved Trainer Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30)
        });

        Assert.Equal(HttpStatusCode.OK, approvedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_TrainerCannotForcePublishedStatus()
    {
        await AuthenticateAsync(email: "force_publish_trainer@example.com", role: "Trainer");

        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_FORCE_PUBLISH",
            Title = "Force Publish Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.Published
        });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("COURSE_REVIEW_REQUIRED", body);
    }

    [Fact]
    public async Task GetAllCourses_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/Course");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var courses = await response.Content.ReadFromJsonAsync<IEnumerable<dynamic>>();
        Assert.NotNull(courses);
    }

    [Fact]
    public async Task SearchAndFilterCourses_ReturnsOk()
    {
        // Act
        var response = await Client.GetAsync("/api/Course/search-and-filter?category=Fitness");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PublicCourseList_ExcludesDraftCourses()
    {
        await AuthenticateAsync(email: "status_trainer@example.com", role: "Trainer");
        var trainerToken = Client.DefaultRequestHeaders.Authorization;
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_STATUS_PUBLISHED",
            Title = "Published Status Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.PendingReview
        });
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_STATUS_DRAFT",
            Title = "Draft Status Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.Draft
        });

        await AuthenticateAsAdminAsync(email: "course-status-admin@example.com");
        var moderationResponse = await Client.PutAsJsonAsync(
            "/api/Course/COURSE_STATUS_PUBLISHED/moderation",
            new UpdateCourseModerationDto { Status = CourseStatus.Published });
        Assert.Equal(HttpStatusCode.OK, moderationResponse.StatusCode);

        Client.DefaultRequestHeaders.Authorization = trainerToken;
        Client.DefaultRequestHeaders.Authorization = null;
        var result = await Client.GetFromJsonAsync<PagedResult<GetCourseDto>>("/api/Course/get-courses?pageSize=50");

        Assert.NotNull(result);
        Assert.Contains(result!.Items, c => c.CourseId == "COURSE_STATUS_PUBLISHED");
        Assert.DoesNotContain(result.Items, c => c.CourseId == "COURSE_STATUS_DRAFT");
    }

    [Fact]
    public async Task GetCourse_DraftCourse_IsVisibleToOwnerOnly()
    {
        await AuthenticateAsync(email: "draft_owner@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_DRAFT_OWNER",
            Title = "Owner Draft Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.Draft
        });

        var ownerResponse = await Client.GetAsync("/api/Course/get-course/COURSE_DRAFT_OWNER");
        Assert.Equal(HttpStatusCode.OK, ownerResponse.StatusCode);

        Client.DefaultRequestHeaders.Authorization = null;
        var publicResponse = await Client.GetAsync("/api/Course/get-course/COURSE_DRAFT_OWNER");
        Assert.Equal(HttpStatusCode.BadRequest, publicResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateCourse_WithRetainedMedia_RemovesUnretainedFiles()
    {
        await AuthenticateAsync(email: "media_owner@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_MEDIA_RETAIN",
            Title = "Media Retain Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            Duration = 10,
            MaxParticipants = 10
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            db.FileCourses.AddRange(
                new FileCourse
                {
                    CourseId = "COURSE_MEDIA_RETAIN",
                    ObjectId = "keep-image.jpg",
                    Url = "/imagecourses/keep-image.jpg",
                    TypeFile = FileType.Image
                },
                new FileCourse
                {
                    CourseId = "COURSE_MEDIA_RETAIN",
                    ObjectId = "remove-image.jpg",
                    Url = "/imagecourses/remove-image.jpg",
                    TypeFile = FileType.Image
                });
            await db.SaveChangesAsync();
        }

        using var form = new MultipartFormDataContent
        {
            { new StringContent("COURSE_MEDIA_RETAIN"), "CourseId" },
            { new StringContent("Media Retain Course Updated"), "Title" },
            { new StringContent("Fitness"), "Type" },
            { new StringContent("Fitness"), "Category" },
            { new StringContent("50"), "Price" },
            { new StringContent("0"), "AdditionalPrice" },
            { new StringContent(DateTime.Now.AddDays(1).ToString("O")), "StartDate" },
            { new StringContent(DateTime.Now.AddDays(30).ToString("O")), "EndDate" },
            { new StringContent("10"), "Duration" },
            { new StringContent("10"), "MaxParticipants" },
            { new StringContent(CourseStatus.PendingReview), "Status" },
            { new StringContent("keep-image.jpg"), "RetainedImageObjectIds" }
        };

        var response = await Client.PutAsync("/api/Course/update-course", form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var verifyScope = Factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var files = await verifyDb.FileCourses
            .Where(f => f.CourseId == "COURSE_MEDIA_RETAIN")
            .ToListAsync();
        Assert.Single(files);
        Assert.Equal("keep-image.jpg", files[0].ObjectId);
    }

    [Fact]
    public async Task ModerateCourse_AsAdmin_PublishesPendingCourse()
    {
        await AuthenticateAsync(email: "moderation_trainer@example.com", role: "Trainer");
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_MODERATION_PENDING",
            Title = "Moderation Pending Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.PendingReview
        });

        Client.DefaultRequestHeaders.Authorization = null;
        var before = await Client.GetAsync("/api/Course/get-course/COURSE_MODERATION_PENDING");
        Assert.Equal(HttpStatusCode.BadRequest, before.StatusCode);

        await AuthenticateAsAdminAsync(email: "moderation-admin@example.com");
        var reviewList = await Client.GetFromJsonAsync<PagedResult<AdminCourseListItemDto>>(
            "/api/Course/admin-review?status=PendingReview&search=Moderation%20Pending");
        Assert.NotNull(reviewList);
        Assert.Contains(reviewList!.Items, c => c.CourseId == "COURSE_MODERATION_PENDING");

        var response = await Client.PutAsJsonAsync(
            "/api/Course/COURSE_MODERATION_PENDING/moderation",
            new UpdateCourseModerationDto { Status = CourseStatus.Published });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Client.DefaultRequestHeaders.Authorization = null;
        var after = await Client.GetAsync("/api/Course/get-course/COURSE_MODERATION_PENDING");
        Assert.Equal(HttpStatusCode.OK, after.StatusCode);
    }
}
