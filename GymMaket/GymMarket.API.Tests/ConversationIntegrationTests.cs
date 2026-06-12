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
        // Arrange — the sender is the authenticated caller; only the receiver is sent.
        await AuthenticateAsync();
        var receiverData = await SignUpAsync("receiver@example.com", "Receiver");

        var model = new CreateConversationDto
        {
            RecieveId = receiverData.UserId!
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Conversations/create-conversation", model);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetConversationOfUser_ReturnsOk()
    {
        // Arrange — the user is derived from the JWT, not the route.
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/Conversations/get-conversation-of-user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMessages_NonMember_ReturnsForbidden()
    {
        // Arrange — the authenticated user starts a conversation with a second user.
        await AuthenticateAsync();
        var receiverData = await SignUpAsync("receiver2@example.com", "Receiver Two");
        await Client.PostAsJsonAsync("/api/Conversations/create-conversation", new CreateConversationDto
        {
            RecieveId = receiverData.UserId!
        });
        var conversations = await Client.GetFromJsonAsync<List<ConversationDto>>("/api/Conversations/get-conversation-of-user");
        var conversationId = conversations!.First().ConversationId;

        // Act — an outsider tries to read the messages.
        await AuthenticateAsync(email: "outsider@example.com");
        var response = await Client.GetAsync($"/api/Conversations/get-messages/{conversationId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Members can still read them.
        await AuthenticateAsync();
        var memberResponse = await Client.GetAsync($"/api/Conversations/get-messages/{conversationId}");
        Assert.Equal(HttpStatusCode.OK, memberResponse.StatusCode);
    }

    private async Task<SignupResponseDto> SignUpAsync(string email, string fullName)
    {
        var password = "Password123";
        var resp = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = fullName,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = "Student"
        });
        return (await resp.Content.ReadFromJsonAsync<SignupResponseDto>())!;
    }
}
