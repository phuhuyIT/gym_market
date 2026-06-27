using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class CourseOptionIntegrationTests : BaseIntegrationTests
{
    public CourseOptionIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateOption_ForOwnedCourse_ReturnsOk_AndPublicCourseOptionsIncludesIt()
    {
        await AuthenticateAsync(email: "option_owner@example.com", role: "Trainer");
        await CreateCourseAsync("OPTION_COURSE_001");

        var response = await Client.PostAsJsonAsync("/api/CourseOption", new CourseOptionCreateDTO
        {
            OptionId = "OPTION_001",
            CourseId = "OPTION_COURSE_001",
            OptionName = "Meal Plan",
            Description = "Custom meal plan",
            Price = 25
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Client.DefaultRequestHeaders.Authorization = null;
        var options = await Client.GetFromJsonAsync<List<CourseOption>>("/api/CourseOption/course/OPTION_COURSE_001");
        Assert.Single(options!);
        Assert.Equal("OPTION_001", options![0].OptionId);
    }

    [Fact]
    public async Task CreateOption_ForAnotherTrainersCourse_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "option_owner2@example.com", role: "Trainer");
        await CreateCourseAsync("OPTION_COURSE_002");

        await AuthenticateAsync(email: "option_intruder@example.com", role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/CourseOption", new CourseOptionCreateDTO
        {
            OptionId = "OPTION_FORBIDDEN",
            CourseId = "OPTION_COURSE_002",
            OptionName = "Private Add-on",
            Description = "Should not be allowed",
            Price = 10
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOptions_AsTrainer_ReturnsOnlyOwnedCourseOptions()
    {
        await AuthenticateAsync(email: "option_list_owner@example.com", role: "Trainer");
        await CreateCourseAsync("OPTION_COURSE_003");
        await Client.PostAsJsonAsync("/api/CourseOption", new CourseOptionCreateDTO
        {
            OptionId = "OPTION_OWNED",
            CourseId = "OPTION_COURSE_003",
            OptionName = "Owned",
            Price = 10
        });

        await AuthenticateAsync(email: "option_list_other@example.com", role: "Trainer");
        await CreateCourseAsync("OPTION_COURSE_004");
        await Client.PostAsJsonAsync("/api/CourseOption", new CourseOptionCreateDTO
        {
            OptionId = "OPTION_OTHER",
            CourseId = "OPTION_COURSE_004",
            OptionName = "Other",
            Price = 10
        });

        var options = await Client.GetFromJsonAsync<List<CourseOption>>("/api/CourseOption");

        Assert.Single(options!);
        Assert.Equal("OPTION_OTHER", options![0].OptionId);
    }

    private async Task CreateCourseAsync(string courseId)
    {
        await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = courseId,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 50,
            MaxParticipants = 10,
            Duration = 10,
            Status = CourseStatus.PendingReview
        });
    }
}
