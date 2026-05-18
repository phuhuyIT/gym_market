using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Course;
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
}
