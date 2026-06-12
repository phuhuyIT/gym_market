using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;

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

    private class RegisterCourseResponseDto
    {
        public string? Message { get; set; }
        public CourseRegistration? Registration { get; set; }
    }
}
