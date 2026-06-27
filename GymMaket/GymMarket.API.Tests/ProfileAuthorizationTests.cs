using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Trainer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class ProfileAuthorizationTests : BaseIntegrationTests
{
    public ProfileAuthorizationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetStudentById_AsDifferentStudent_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "student-profile-owner@example.com", role: "Student");
        var ownerStudentId = GetTokenClaim("studentId");

        await AuthenticateAsync(email: "student-profile-attacker@example.com", role: "Student");
        var response = await Client.GetAsync($"/api/Student/{ownerStudentId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetStudentById_AsOwner_ReturnsOk()
    {
        await AuthenticateAsync(email: "student-profile-self@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId");

        var response = await Client.GetAsync($"/api/Student/{studentId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTrainerList_AsStudent_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "trainer-list-student@example.com", role: "Student");

        var response = await Client.GetAsync("/api/Trainer");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetTrainerById_AsStudent_DoesNotExposeBankDetails()
    {
        await AuthenticateAsync(email: "bank-detail-trainer@example.com", role: "Trainer");
        var trainerId = GetTokenClaim("trainerId")!;

        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var trainer = await context.Trainers.FindAsync(trainerId);
            trainer!.BankBin = "970436";
            trainer.BankAccountNo = "secret-account-number";
            trainer.BankAccountName = "Secret Trainer Account";
            await context.SaveChangesAsync();
        }

        await AuthenticateAsync(email: "bank-detail-student@example.com", role: "Student");
        var response = await Client.GetAsync($"/api/Trainer/{trainerId}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("bankBin", body);
        Assert.DoesNotContain("bankAccountNo", body);
        Assert.DoesNotContain("bankAccountName", body);
        Assert.DoesNotContain("secret-account-number", body);
    }

    [Fact]
    public async Task UpdateTrainer_AsDifferentTrainer_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "trainer-profile-owner@example.com", role: "Trainer");
        var ownerTrainerId = GetTokenClaim("trainerId")!;
        var ownerUserId = GetTokenClaim("nameid")!;

        await AuthenticateAsync(email: "trainer-profile-attacker@example.com", role: "Trainer");
        var update = new TrainerUpdateDTO
        {
            TrainerId = ownerTrainerId,
            UserId = ownerUserId,
            Name = "Updated Owner",
            Email = "trainer-profile-owner@example.com",
            Certification = "Updated Certification",
            Category = "Strength",
            Bio = "Updated bio",
            Experience = 5,
            Rating = 4,
            ProfilePicture = "https://example.com/trainer.png",
            UpdatedAt = DateTime.UtcNow
        };

        var response = await Client.PutAsJsonAsync($"/api/Trainer/{ownerTrainerId}", update);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
