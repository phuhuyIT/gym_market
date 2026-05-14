using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class AccountsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AccountsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace real database with in-memory for tests
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GymMarketContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<GymMarketContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SignUp_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var model = new SignUpDto
        {
            FullName = "Integration Test User",
            Email = "int_test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            Role = "Student"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Accounts/sign-up", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        // Arrange
        var email = "login_test@example.com";
        var password = "Password123";
        
        // First, sign up the user
        await _client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Login Test User",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });

        var loginModel = new LoginDto
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Accounts/login", loginModel);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsBadRequest()
    {
        // Arrange
        var email = "wrong_pass@example.com";
        await _client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Wrong Pass User",
            Email = email,
            Password = "Password123",
            ConfirmPassword = "Password123",
            Role = "Student"
        });

        var loginModel = new LoginDto
        {
            Email = email,
            Password = "WrongPassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Accounts/login", loginModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
