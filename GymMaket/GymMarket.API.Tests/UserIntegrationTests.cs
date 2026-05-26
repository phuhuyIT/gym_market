using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.User;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class UserIntegrationTests : BaseIntegrationTests
{
    public UserIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetUserInfo_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var email = "userinfo@example.com";
        var password = "Password123";
        var signupResp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Info User",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        var signupData = await signupResp.Content.ReadFromJsonAsync<SignupResponseDto>();
        var userId = signupData!.UserId;

        // Act
        var response = await Client.GetAsync($"/api/Users/get-user-info/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync();
        var email = "updateuser@example.com";
        var password = "Password123";
        var signupResp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Original Name",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        var signupData = await signupResp.Content.ReadFromJsonAsync<SignupResponseDto>();
        var userId = signupData!.UserId;

        var updateModel = new UpdateUserDto
        {
            Id = userId!,
            FullName = "Updated Name",
            PhoneNumber = "123456789",
            Address = "Test Address",
            Avatar = "https://example.com/avatar.png"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/Users/update-user", updateModel);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}
