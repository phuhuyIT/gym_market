using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Response;
using Microsoft.AspNetCore.Mvc.Testing;

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
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_STATUS_PUBLISHED",
            Title = "Published Status Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.Published
        });
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = "COURSE_STATUS_DRAFT",
            Title = "Draft Status Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Status = CourseStatus.Draft
        });

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
}
