using System.Net.Http.Headers;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class BaseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    public BaseIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var dbName = Guid.NewGuid().ToString();
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace real database with in-memory for tests
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GymMarketContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<GymMarketContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });

        Client = Factory.CreateClient();
    }

    protected async Task AuthenticateAsync(string email = "test@example.com", string password = "Password123", string role = "Student")
    {
        // Sign up if not already exists (in-memory db is fresh for each class fixture, but we might want to ensure user exists)
        await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Test User",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = role
        });

        var loginResponse = await Client.PostAsJsonAsync("/api/Accounts/login", new LoginDto
        {
            Email = email,
            Password = password
        });

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    // Helper DTO for login result if not already available in the project
    public class LoginResultDto
    {
        public string Token { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class SignupResponseDto
    {
        public string? UserId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
