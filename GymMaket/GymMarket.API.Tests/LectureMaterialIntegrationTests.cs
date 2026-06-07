using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.DTOs.LectureMaterial;
using GymMarket.API.DTOs.Course;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class LectureMaterialIntegrationTests : BaseIntegrationTests
{
    public LectureMaterialIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    private async Task<string> CreateTestLectureAsync()
    {
        await AuthenticateAsync(role: "Trainer");
        var courseId = "COURSE_" + Guid.NewGuid().ToString().Substring(0, 8);
        var courseModel = new CourseCreateDTO
        {
            CourseId = courseId,
            // Own the course so the trainer is authorized to manage its lectures/materials.
            TrainerId = GetTokenClaim("trainerId"),
            Title = "Test Course for Material",
            Description = "A course for testing materials",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Duration = 4,
            MaxParticipants = 20
        };
        await Client.PostAsJsonAsync("/api/Course", courseModel);

        var lectureId = "LECTURE_" + Guid.NewGuid().ToString().Substring(0, 8);
        var lectureModel = new LectureCreateDTO
        {
            LectureId = lectureId,
            CourseId = courseId,
            Title = "Test Lecture for Material",
            Order = 1,
            Duration = 60
        };
        await Client.PostAsJsonAsync("/api/Lecture", lectureModel);
        return lectureId;
    }

    [Fact]
    public async Task CreateMaterial_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var lectureId = await CreateTestLectureAsync();
        var model = new LectureMaterialCreateDTO
        {
            MaterialId = "MATERIAL_001",
            LectureId = lectureId,
            MaterialType = "Video",
            MaterialContent = "https://example.com/video.mp4"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/LectureMaterial", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMaterialsByLectureId_ReturnsOk()
    {
        // Arrange
        var lectureId = await CreateTestLectureAsync();
        var model = new LectureMaterialCreateDTO
        {
            MaterialId = "MATERIAL_002",
            LectureId = lectureId,
            MaterialType = "PDF",
            MaterialContent = "https://example.com/doc.pdf"
        };
        await Client.PostAsJsonAsync("/api/LectureMaterial", model);

        // Act
        var response = await Client.GetAsync($"/api/LectureMaterial/lecture/{lectureId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var materials = await response.Content.ReadFromJsonAsync<IEnumerable<GetLectureMaterialDto>>();
        Assert.NotNull(materials);
        Assert.Contains(materials, m => m.MaterialId == "MATERIAL_002");
    }

    [Fact]
    public async Task UpdateMaterial_ReturnsNoContent()
    {
        // Arrange
        var lectureId = await CreateTestLectureAsync();
        var materialId = "MATERIAL_003";
        var createModel = new LectureMaterialCreateDTO
        {
            MaterialId = materialId,
            LectureId = lectureId,
            MaterialType = "Text",
            MaterialContent = "Original Content"
        };
        await Client.PostAsJsonAsync("/api/LectureMaterial", createModel);

        var updateModel = new LectureMaterialUpdateDTO
        {
            MaterialId = materialId,
            LectureId = lectureId,
            MaterialType = "Text",
            MaterialContent = "Updated Content"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/LectureMaterial/{materialId}", updateModel);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMaterial_ReturnsNoContent()
    {
        // Arrange
        var lectureId = await CreateTestLectureAsync();
        var materialId = "MATERIAL_004";
        var model = new LectureMaterialCreateDTO
        {
            MaterialId = materialId,
            LectureId = lectureId,
            MaterialType = "Image",
            MaterialContent = "https://example.com/image.png"
        };
        await Client.PostAsJsonAsync("/api/LectureMaterial", model);

        // Act
        var response = await Client.DeleteAsync($"/api/LectureMaterial/{materialId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
