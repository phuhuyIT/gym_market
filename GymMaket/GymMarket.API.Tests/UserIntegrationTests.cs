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
    public async Task GetUserInfo_ReturnsOwnProfile()
    {
        // Arrange — the endpoint serves the caller's own profile (id from JWT).
        await AuthenticateAsync(email: "userinfo@example.com");

        // Act
        var response = await Client.GetAsync("/api/Users/get-user-info");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var info = await response.Content.ReadFromJsonAsync<UserInfoResponseDto>();
        Assert.Equal(GetTokenClaim("nameid"), info!.UserInfo!.Id);
        Assert.Equal("userinfo@example.com", info.UserInfo.Email);
    }

    [Fact]
    public async Task UpdateUser_UpdatesTheAuthenticatedUserOnly()
    {
        // Arrange — a second (potential victim) account exists and is left untouched.
        await AuthenticateAsync(email: "victim@example.com");
        var victimUpdate = new UpdateUserDto
        {
            FullName = "Victim Original",
            PhoneNumber = "000",
            Address = "Victim Address",
            Avatar = "https://example.com/victim.png"
        };
        await Client.PutAsJsonAsync("/api/Users/update-user", victimUpdate);

        // The attacker authenticates and updates "their" profile. The DTO carries no
        // id, so the change can only ever land on the caller.
        await AuthenticateAsync(email: "attacker@example.com");
        var updateModel = new UpdateUserDto
        {
            FullName = "Updated Name",
            PhoneNumber = "123456789",
            Address = "Test Address",
            Avatar = "https://example.com/avatar.png"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/Users/update-user", updateModel);

        // Assert — the attacker's own profile changed…
        Assert.True(response.IsSuccessStatusCode);
        var attackerInfo = await Client.GetFromJsonAsync<UserInfoResponseDto>("/api/Users/get-user-info");
        Assert.Equal("Updated Name", attackerInfo!.UserInfo!.FullName);

        // …and the victim's profile (read as the victim) was not affected.
        await AuthenticateAsync(email: "victim@example.com");
        var victimInfo = await Client.GetFromJsonAsync<UserInfoResponseDto>("/api/Users/get-user-info");
        Assert.Equal("Victim Original", victimInfo!.UserInfo!.FullName);
    }

    private class UserInfoResponseDto
    {
        public GetUserInfoDto? UserInfo { get; set; }
    }
}
