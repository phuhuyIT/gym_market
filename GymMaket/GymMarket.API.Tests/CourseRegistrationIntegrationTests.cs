using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.Student;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class CourseRegistrationIntegrationTests : BaseIntegrationTests
{
    public CourseRegistrationIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterCourse_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync();

        // 1. Create a Student
        var studentEmail = "student_reg@example.com";
        var password = "Password123";
        var signupResp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Reg Student",
            Email = studentEmail,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        var signupData = await signupResp.Content.ReadFromJsonAsync<SignupResponseDto>();
        var userId = signupData!.UserId;

        var studentCreate = new StudentCreateDTO
        {
            UserId = userId,
            Name = "Reg Student",
            Email = studentEmail,
            Password = password,
            HealthStatus = "Healthy"
        };
        await Client.PostAsJsonAsync("/api/Student", studentCreate);

        // 2. Create a Course
        var courseCreate = new CourseCreateDTO
        {
            CourseId = "CRS_REG_001",
            Title = "Registration Test Course",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50
        };
        await Client.PostAsJsonAsync("/api/Course", courseCreate);

        // 3. Register
        var registerDto = new RegisterCourseDto
        {
            CourseId = "CRS_REG_001",
            StudentId = "STU_001"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/CourseRegistration/register-course", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
