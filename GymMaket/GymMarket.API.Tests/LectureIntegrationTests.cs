using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.DTOs.Course;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class LectureIntegrationTests : BaseIntegrationTests
{
    public LectureIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    private async Task<string> CreateTestCourseAsync()
    {
        await AuthenticateAsync(role: "Trainer");
        var courseId = "COURSE_" + Guid.NewGuid().ToString().Substring(0, 8);
        var courseModel = new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Test Course for Lecture",
            Description = "A course for testing lectures",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Duration = 4,
            MaxParticipants = 20
        };
        await Client.PostAsJsonAsync("/api/Course", courseModel);
        return courseId;
    }

    [Fact]
    public async Task CreateLecture_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var courseId = await CreateTestCourseAsync();
        var model = new LectureCreateDTO
        {
            LectureId = "LECTURE_001",
            CourseId = courseId,
            Title = "Test Lecture",
            Order = 1,
            Duration = 60
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Lecture", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLecturesByCourseId_ReturnsOk()
    {
        // Arrange
        var courseId = await CreateTestCourseAsync();
        var model = new LectureCreateDTO
        {
            LectureId = "LECTURE_002",
            CourseId = courseId,
            Title = "Test Lecture 2",
            Order = 1,
            Duration = 60
        };
        await Client.PostAsJsonAsync("/api/Lecture", model);

        // Act
        var response = await Client.GetAsync($"/api/Lecture/course/{courseId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var lectures = await response.Content.ReadFromJsonAsync<IEnumerable<GetLectureDto>>();
        Assert.NotNull(lectures);
        Assert.Contains(lectures, l => l.LectureId == "LECTURE_002");
    }

    [Fact]
    public async Task UpdateLecture_ReturnsNoContent()
    {
        // Arrange
        var courseId = await CreateTestCourseAsync();
        var lectureId = "LECTURE_003";
        var createModel = new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Original Title",
            Order = 1,
            Duration = 60
        };
        await Client.PostAsJsonAsync("/api/Lecture", createModel);

        var updateModel = new LectureUpdateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Updated Title",
            Order = 2,
            Duration = 90
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/Lecture/{lectureId}", updateModel);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLecture_ReturnsNoContent()
    {
        // Arrange
        var courseId = await CreateTestCourseAsync();
        var lectureId = "LECTURE_004";
        var model = new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "To Be Deleted",
            Order = 1,
            Duration = 60
        };
        await Client.PostAsJsonAsync("/api/Lecture", model);

        // Act
        var response = await Client.DeleteAsync($"/api/Lecture/{lectureId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
