using System.Net.Http.Headers;
using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
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

    // Sign-up rejects the Admin role by design (ApplicationRoles.All), so register
    // normally, grant the role directly, then re-login for a token with the role claim.
    protected async Task AuthenticateAsAdminAsync(string email = "admin@example.com", string password = "Password123")
    {
        await AuthenticateAsync(email, password);

        using (var scope = Factory.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByEmailAsync(email);
            if (!await userManager.IsInRoleAsync(user!, "Admin"))
            {
                await userManager.AddToRoleAsync(user!, "Admin");
            }
        }

        var loginResponse = await Client.PostAsJsonAsync("/api/Accounts/login", new LoginDto
        {
            Email = email,
            Password = password
        });

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    // Reads a claim out of the bearer token currently on the client (e.g. "trainerId",
    // "studentId"). Used so tests can act as the owner of the resources they create.
    protected string? GetTokenClaim(string claimType)
    {
        var token = Client.DefaultRequestHeaders.Authorization?.Parameter;
        if (string.IsNullOrWhiteSpace(token)) return null;

        var parts = token.Split('.');
        if (parts.Length < 2) return null;

        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        payload = (payload.Length % 4) switch
        {
            2 => payload + "==",
            3 => payload + "=",
            _ => payload
        };

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty(claimType, out var value) ? value.GetString() : null;
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
