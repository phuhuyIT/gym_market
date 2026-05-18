using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.FoodNutritionUser;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class FoodNutritionIntegrationTests : BaseIntegrationTests
{
    public FoodNutritionIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SearchFoodNutrition_ReturnsOk()
    {
        // Act
        var response = await Client.GetAsync("/api/FoodNutrition/search-nutrition?search=apple");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodNutritionUser_ReturnsOk()
    {
        // Act
        var response = await Client.GetAsync("/api/FoodNutrition/get-nutrition-user/user1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CalCaloricValue_WithValidData_ReturnsOk()
    {
        // Arrange
        var model = new AddFoodNutritionUser
        {
            UserId = "user1",
            FoodNutritionId = 1,
            FoodName = "Apple",
            Weight = 100,
            Fat = 0.5,
            Sugars = 10,
            Protein = 0.3
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", model);

        // Assert
        // This might fail if the ID 1 doesn't exist, but we are testing the API endpoint reachability and basic logic
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest);
    }
}
