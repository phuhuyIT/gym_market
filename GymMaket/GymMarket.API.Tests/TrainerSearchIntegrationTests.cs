using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class TrainerSearchIntegrationTests : BaseIntegrationTests
{
    public TrainerSearchIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SearchTrainer_WithCategory_UsesCategoryAndFallbackText()
    {
        // Arrange
        await AuthenticateAsync();
        await SeedTrainersAsync();

        // Act
        var result = await Client.GetFromJsonAsync<PagedResult<TrainerSearchDto>>(
            "/api/Trainer/search?category=Yoga&pageIndex=1&pageSize=10");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.TotalCount);
        Assert.Equal(["TR_YOGA_CATEGORY", "TR_YOGA_FALLBACK"], result.Items.Select(t => t.TrainerId).Order().ToArray());
        Assert.Contains(result.Items, t => t.Category == "Yoga");
    }

    [Fact]
    public async Task SearchTrainer_WithEliteOnly_ReturnsExperiencedTrainers()
    {
        // Arrange
        await AuthenticateAsync();
        await SeedTrainersAsync();

        // Act
        var result = await Client.GetFromJsonAsync<PagedResult<TrainerSearchDto>>(
            "/api/Trainer/search?eliteOnly=true&pageIndex=1&pageSize=10");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.TotalCount);
        Assert.All(result.Items, trainer => Assert.True(trainer.Experience >= 8));
    }

    [Fact]
    public async Task SearchTrainer_WithSearchCategoryEliteAndPaging_ReturnsPagedFilteredResults()
    {
        // Arrange
        await AuthenticateAsync();
        await SeedTrainersAsync();

        // Act
        var pageOne = await Client.GetFromJsonAsync<PagedResult<TrainerSearchDto>>(
            "/api/Trainer/search?search=coach&category=Strength&eliteOnly=true&pageIndex=1&pageSize=1");
        var pageTwo = await Client.GetFromJsonAsync<PagedResult<TrainerSearchDto>>(
            "/api/Trainer/search?search=coach&category=Strength&eliteOnly=true&pageIndex=2&pageSize=1");

        // Assert
        Assert.NotNull(pageOne);
        Assert.NotNull(pageTwo);
        Assert.Equal(2, pageOne!.TotalCount);
        Assert.Equal(2, pageOne.TotalPages);
        Assert.Single(pageOne.Items);
        Assert.True(pageOne.HasNextPage);
        Assert.False(pageOne.HasPreviousPage);
        Assert.Single(pageTwo!.Items);
        Assert.True(pageTwo.HasPreviousPage);
        Assert.False(pageTwo.HasNextPage);
        Assert.NotEqual(pageOne.Items[0].TrainerId, pageTwo.Items[0].TrainerId);
    }

    private async Task SeedTrainersAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();

        context.Trainers.AddRange(
            new Trainer
            {
                TrainerId = "TR_YOGA_CATEGORY",
                Name = "Yoga Coach",
                Email = "yoga-category@example.com",
                Category = "Yoga",
                Certification = "Mobility",
                Bio = "Calm flexibility coach",
                Description = "Mindful movement",
                Experience = 5,
                Rating = 4.7m
            },
            new Trainer
            {
                TrainerId = "TR_YOGA_FALLBACK",
                Name = "Fallback Coach",
                Email = "yoga-fallback@example.com",
                Certification = "Certified Yoga Instructor",
                Bio = "Legacy profile without category",
                Description = "Restorative sessions",
                Experience = 4,
                Rating = 4.5m
            },
            new Trainer
            {
                TrainerId = "TR_STRENGTH_A",
                Name = "Alpha Coach",
                Email = "strength-a@example.com",
                Category = "Strength",
                Certification = "Powerlifting",
                Bio = "Strength coach",
                Description = "Barbell programming",
                Experience = 10,
                Rating = 4.9m
            },
            new Trainer
            {
                TrainerId = "TR_STRENGTH_B",
                Name = "Beta Coach",
                Email = "strength-b@example.com",
                Category = "Strength",
                Certification = "Hypertrophy",
                Bio = "Strength coach",
                Description = "Muscle building",
                Experience = 8,
                Rating = 4.8m
            },
            new Trainer
            {
                TrainerId = "TR_CARDIO",
                Name = "Cardio Coach",
                Email = "cardio@example.com",
                Category = "Cardio",
                Certification = "Endurance",
                Bio = "Running coach",
                Description = "Conditioning",
                Experience = 6,
                Rating = 4.4m
            });

        await context.SaveChangesAsync();
    }
}
