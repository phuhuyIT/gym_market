using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.UserMessage;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class ConversationIntegrationTests : BaseIntegrationTests
{
    public ConversationIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateConversation_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync();
        var senderEmail = "sender@example.com";
        var receiverEmail = "receiver@example.com";
        var password = "Password123";

        // Sign up sender
        var senderResp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Sender",
            Email = senderEmail,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        var senderData = await senderResp.Content.ReadFromJsonAsync<SignupResponseDto>();

        // Sign up receiver
        var receiverResp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = "Receiver",
            Email = receiverEmail,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        var receiverData = await receiverResp.Content.ReadFromJsonAsync<SignupResponseDto>();

        var model = new CreateConversationDto
        {
            SenderId = senderData!.UserId!,
            RecieveId = receiverData!.UserId!
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Conversations/create-conversation", model);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetConversationOfUser_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/Conversations/get-conversation-of-user/user1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
