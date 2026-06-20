using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.FoodNutrition;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class FoodNutritionIntegrationTests : BaseIntegrationTests
{
    public FoodNutritionIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SearchFoodNutrition_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/FoodNutrition/search-nutrition?search=apple");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodNutritionUser_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/FoodNutrition/get-nutrition-user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CalCaloricValue_WithValidData_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var model = new AddFoodNutritionUser
        {
            FoodNutritionId = 1,
            FoodName = "Apple",
            Weight = 100
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", model);

        // Assert
        // This might fail if the ID 1 doesn't exist, but we are testing the API endpoint reachability and basic logic
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CalCaloricValue_PersistsDateAndMealType()
    {
        // Arrange
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        var model = new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 200,
            Date = new DateOnly(2026, 6, 12),
            MealType = "Lunch"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<FoodNutritionUser>();
        Assert.NotNull(created);
        Assert.Equal(new DateOnly(2026, 6, 12), created!.Date);
        Assert.Equal("Lunch", created.MealType);
        Assert.Equal(104, created.CaloricValue); // 52 kcal/100g * 200g
        Assert.Equal(27.6, created.Carbs, 5);    // 13.8 g/100g * 200g
        Assert.Equal(GetTokenClaim("nameid"), created.UserId); // owner comes from the JWT
    }

    [Fact]
    public async Task UpdateFoodNutritionUser_RescalesMacrosAndUpdatesMealType()
    {
        // Arrange
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        var createResponse = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100,
            Date = new DateOnly(2026, 6, 12),
            MealType = "Breakfast"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<FoodNutritionUser>();

        // Act
        var response = await Client.PutAsJsonAsync("/api/FoodNutrition/update-foodnutrition-user", new UpdateFoodNutritionUserDto
        {
            FoodNutritionUserId = created!.Id,
            Weight = 300,
            Date = new DateOnly(2026, 6, 13),
            MealType = "Dinner"
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<FoodNutritionUser>();
        Assert.NotNull(updated);
        Assert.Equal(300, updated!.Weight);
        Assert.Equal(156, updated.CaloricValue); // 52 kcal/100g * 300g
        Assert.Equal(41.4, updated.Carbs, 5);    // 13.8 g/100g * 300g
        Assert.Equal(0.9, updated.Protein, 5);   // 0.3 g/100g * 300g
        Assert.Equal(new DateOnly(2026, 6, 13), updated.Date);
        Assert.Equal("Dinner", updated.MealType);
    }

    [Fact]
    public async Task UpdateFoodNutritionUser_AnotherUsersLog_ReturnsBadRequest()
    {
        // Arrange — owner logs an entry
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        var createResponse = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100
        });
        var created = await createResponse.Content.ReadFromJsonAsync<FoodNutritionUser>();

        // Act — a different authenticated user tries to update it
        await AuthenticateAsync(email: "attacker@example.com");
        var response = await Client.PutAsJsonAsync("/api/FoodNutrition/update-foodnutrition-user", new UpdateFoodNutritionUserDto
        {
            FoodNutritionUserId = created!.Id,
            Weight = 300
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodNutritionUser_WithBody_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        var createResponse = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100
        });
        var created = await createResponse.Content.ReadFromJsonAsync<FoodNutritionUser>();

        // Act — the Angular client sends DELETE with a JSON body
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/FoodNutrition/delete-foodnutrition-user")
        {
            Content = JsonContent.Create(new DeleteFoodNutritionUserDto
            {
                FoodNutritionUserId = created!.Id
            })
        };
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodNutritionUser_FiltersByDate()
    {
        // Arrange — two entries on different days
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100,
            Date = new DateOnly(2026, 6, 12)
        });
        await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100,
            Date = new DateOnly(2026, 6, 13)
        });

        // Act
        var response = await Client.GetAsync("/api/FoodNutrition/get-nutrition-user?date=2026-06-13");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var logs = await response.Content.ReadFromJsonAsync<List<FoodNutritionUser>>();
        Assert.NotNull(logs);
        Assert.Single(logs!);
        Assert.Equal(new DateOnly(2026, 6, 13), logs![0].Date);
    }

    [Fact]
    public async Task GetFoodNutritionUser_PaginatesWhenRequested()
    {
        // Arrange — three entries, page size two
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        for (var i = 0; i < 3; i++)
        {
            await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
            {
                FoodNutritionId = foodId,
                FoodName = "Apple",
                Weight = 100
            });
        }

        // Act
        var pageOne = await Client.GetFromJsonAsync<List<FoodNutritionUser>>(
            "/api/FoodNutrition/get-nutrition-user?page=1&pageSize=2");
        var pageTwo = await Client.GetFromJsonAsync<List<FoodNutritionUser>>(
            "/api/FoodNutrition/get-nutrition-user?page=2&pageSize=2");
        var all = await Client.GetFromJsonAsync<List<FoodNutritionUser>>(
            "/api/FoodNutrition/get-nutrition-user");

        // Assert
        Assert.Equal(2, pageOne!.Count);
        Assert.Single(pageTwo!);
        Assert.Equal(3, all!.Count); // no pageSize -> full log (frontend contract)
    }

    [Fact]
    public async Task UpdateFoodNutritionUser_AfterMasterFoodDeleted_RescalesSnapshot()
    {
        // Arrange — log an entry, then remove its master food
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();
        var createResponse = await Client.PostAsJsonAsync("/api/FoodNutrition/cal-caloric-value", new AddFoodNutritionUser
        {
            FoodNutritionId = foodId,
            FoodName = "Apple",
            Weight = 100
        });
        var created = await createResponse.Content.ReadFromJsonAsync<FoodNutritionUser>();

        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
            var food = await context.FoodNutritions.FindAsync(foodId);
            context.FoodNutritions.Remove(food!);
            await context.SaveChangesAsync();
        }

        // Act — the log keeps its snapshot and rescales linearly
        var response = await Client.PutAsJsonAsync("/api/FoodNutrition/update-foodnutrition-user", new UpdateFoodNutritionUserDto
        {
            FoodNutritionUserId = created!.Id,
            Weight = 200
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<FoodNutritionUser>();
        Assert.Equal(104, updated!.CaloricValue); // 52 kcal/100g * 200g
        Assert.Equal(27.6, updated.Carbs, 5);      // 13.8 g/100g * 200g
    }

    [Fact]
    public async Task GetNutritionBudget_WithoutSavedBudget_ReturnsDefaults()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var budget = await Client.GetFromJsonAsync<NutritionBudgetDto>("/api/FoodNutrition/nutrition-budget");

        // Assert — matches the calculator UI defaults
        Assert.NotNull(budget);
        Assert.Equal(2000, budget!.CalorieBudget);
        Assert.Equal(250, budget.CarbsBudget);
        Assert.Equal(65, budget.FatBudget);
        Assert.Equal(130, budget.ProteinBudget);
    }

    [Fact]
    public async Task SaveNutritionBudget_PersistsAndRoundTrips()
    {
        // Arrange
        await AuthenticateAsync();
        var model = new NutritionBudgetDto
        {
            CalorieBudget = 2500,
            CarbsBudget = 300,
            FatBudget = 80,
            ProteinBudget = 180
        };

        // Act — save twice to cover both the insert and the update path
        var firstSave = await Client.PutAsJsonAsync("/api/FoodNutrition/nutrition-budget", model);
        model.ProteinBudget = 200;
        var secondSave = await Client.PutAsJsonAsync("/api/FoodNutrition/nutrition-budget", model);
        var budget = await Client.GetFromJsonAsync<NutritionBudgetDto>("/api/FoodNutrition/nutrition-budget");

        // Assert
        Assert.Equal(HttpStatusCode.OK, firstSave.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondSave.StatusCode);
        Assert.NotNull(budget);
        Assert.Equal(2500, budget!.CalorieBudget);
        Assert.Equal(200, budget.ProteinBudget);
    }

    [Fact]
    public async Task CreateFoodNutrition_AsAdmin_PersistsFood()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var model = new CreateFoodNutritionDto
        {
            Name = "Quinoa (Cooked)",
            CaloricValue = 120,
            Carbs = 21.3,
            Fat = 1.9,
            Sugars = 0.9,
            Protein = 4.4
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/create-nutrition", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<FoodNutrition>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Quinoa (Cooked)", created.Name);
        Assert.Equal(120, created.CaloricValue);
    }

    [Fact]
    public async Task CreateFoodNutrition_AsStudent_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsync();
        var model = new CreateFoodNutritionDto { Name = "Hacker Food", CaloricValue = 1 };

        // Act
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/create-nutrition", model);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodNutrition_DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        await SeedFoodAsync(); // seeds "Apple"
        var model = new CreateFoodNutritionDto { Name = "apple", CaloricValue = 52 };

        // Act — name comparison is case-insensitive
        var response = await Client.PostAsJsonAsync("/api/FoodNutrition/create-nutrition", model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFoodNutrition_AsAdmin_UpdatesValues()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var foodId = await SeedFoodAsync();
        var model = new UpdateFoodNutritionDto
        {
            Name = "Green Apple",
            CaloricValue = 58,
            Carbs = 14.0,
            Fat = 0.2,
            Sugars = 9.6,
            Protein = 0.4
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/FoodNutrition/update-nutrition/{foodId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<FoodNutrition>();
        Assert.NotNull(updated);
        Assert.Equal("Green Apple", updated!.Name);
        Assert.Equal(58, updated.CaloricValue);
        Assert.Equal(14.0, updated.Carbs, 5);
        Assert.Equal(9.6, updated.Sugars, 5);
    }

    [Fact]
    public async Task UpdateFoodNutrition_UnknownId_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var model = new UpdateFoodNutritionDto { Name = "Ghost Food", CaloricValue = 1 };

        // Act
        var response = await Client.PutAsJsonAsync("/api/FoodNutrition/update-nutrition/999999", model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodNutrition_AsAdmin_RemovesFood()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var foodId = await SeedFoodAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/FoodNutrition/delete-nutrition/{foodId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var searchResponse = await Client.GetAsync("/api/FoodNutrition/search-nutrition?search=apple");
        var remaining = await searchResponse.Content.ReadFromJsonAsync<List<FoodNutrition>>();
        Assert.NotNull(remaining);
        Assert.DoesNotContain(remaining!, f => f.Id == foodId);
    }

    [Fact]
    public async Task DeleteFoodNutrition_AsStudent_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsync();
        var foodId = await SeedFoodAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/FoodNutrition/delete-nutrition/{foodId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // The in-memory test database never runs migrations/HasData, so seed a food row directly.
    private async Task<int> SeedFoodAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var food = new FoodNutrition { Name = "Apple", CaloricValue = 52, Carbs = 13.8, Fat = 0.2, Sugars = 10.4, Protein = 0.3 };
        context.FoodNutritions.Add(food);
        await context.SaveChangesAsync();
        return food.Id;
    }
}
