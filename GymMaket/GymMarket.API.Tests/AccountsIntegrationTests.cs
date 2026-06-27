using System.Net;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Identity;
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

                foreach (var emailSender in services.Where(s => s.ServiceType == typeof(IEmailSender)).ToList())
                {
                    services.Remove(emailSender);
                }
                services.AddSingleton<IEmailSender, NoopEmailSender>();
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
        await ConfirmUserEmailAsync(email);

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
    public async Task Login_WithUnconfirmedEmail_ReturnsBadRequest()
    {
        var email = "unconfirmed_login@example.com";
        var password = "Password123";

        await _client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Unconfirmed Login User",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });

        var response = await _client.PostAsJsonAsync("/api/Accounts/login", new LoginDto
        {
            Email = email,
            Password = password
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("EMAIL_NOT_CONFIRMED", body);
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

    private async Task ConfirmUserEmailAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
    }

    private sealed class NoopEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string htmlBody) => Task.CompletedTask;
    }
}
