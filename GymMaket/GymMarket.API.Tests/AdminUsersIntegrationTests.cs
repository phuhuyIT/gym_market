using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Response;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class AdminUsersIntegrationTests : BaseIntegrationTests
{
    public AdminUsersIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Search_AsStudent_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "admin-users-student@example.com", role: "Student");

        var response = await Client.GetAsync("/api/Admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Search_AsAdmin_ReturnsUsersWithRolesAndProfiles()
    {
        await AuthenticateAsAdminAsync(email: "admin-users-admin@example.com");
        var targetEmail = "admin-users-trainer@example.com";
        await SignUpAsync(targetEmail, "Admin Users Trainer", "Trainer");

        var result = await Client.GetFromJsonAsync<PagedResult<AdminUserListItemDto>>(
            "/api/Admin/users?role=Trainer&search=Admin%20Users%20Trainer");

        Assert.NotNull(result);
        var user = Assert.Single(result!.Items);
        Assert.Equal(targetEmail, user.Email);
        Assert.Contains("Trainer", user.Roles);
        Assert.NotNull(user.TrainerId);
        Assert.Equal(TrainerApprovalStatus.PendingReview, user.TrainerApprovalStatus);
    }

    [Fact]
    public async Task UpdateTrainerApproval_AsAdmin_ApprovesTrainer()
    {
        await AuthenticateAsAdminAsync(email: "admin-users-approval-admin@example.com");
        var targetEmail = "admin-users-approval-trainer@example.com";
        var signup = await SignUpAsync(targetEmail, "Approval Trainer", "Trainer");

        var updateResponse = await Client.PutAsJsonAsync(
            $"/api/Admin/users/{signup.UserId}/trainer-approval",
            new UpdateTrainerApprovalDto { Status = TrainerApprovalStatus.Approved });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var detail = await Client.GetFromJsonAsync<AdminUserDetailDto>($"/api/Admin/users/{signup.UserId}");
        Assert.NotNull(detail);
        Assert.Equal(TrainerApprovalStatus.Approved, detail!.TrainerApprovalStatus);
    }

    [Fact]
    public async Task ResendConfirmation_AsAdmin_ReturnsOkForUnconfirmedUser()
    {
        await AuthenticateAsAdminAsync(email: "admin-users-resend-admin@example.com");
        var signup = await SignUpAsync("admin-users-unconfirmed@example.com", "Unconfirmed User", "Student");

        var response = await Client.PostAsync($"/api/Admin/users/{signup.UserId}/resend-confirmation", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_ToSuspended_BlocksFutureLogin()
    {
        await AuthenticateAsAdminAsync(email: "admin-users-suspend-admin@example.com");
        var targetEmail = "admin-users-suspended@example.com";
        var password = "Password123";
        var signup = await SignUpAsync(targetEmail, "Suspended User", "Student", password);
        await ConfirmUserEmailAsync(targetEmail);

        var updateResponse = await Client.PutAsJsonAsync(
            $"/api/Admin/users/{signup.UserId}/status",
            new UpdateUserStatusDto { Status = "Suspended" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var loginResponse = await Client.PostAsJsonAsync("/api/Accounts/login", new LoginDto
        {
            Email = targetEmail,
            Password = password
        });
        var body = await loginResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
        Assert.Contains("ACCOUNT_SUSPENDED", body);
    }

    [Fact]
    public async Task UpdateStatus_ToSuspended_InvalidatesExistingAccessToken()
    {
        await AuthenticateAsync(email: "admin-users-token-target@example.com", role: "Student");
        var targetId = GetTokenClaim("nameid")!;
        var oldToken = Client.DefaultRequestHeaders.Authorization!.Parameter!;

        await AuthenticateAsAdminAsync(email: "admin-users-token-admin@example.com");
        var updateResponse = await Client.PutAsJsonAsync(
            $"/api/Admin/users/{targetId}/status",
            new UpdateUserStatusDto { Status = "Suspended" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oldToken);
        var protectedResponse = await Client.GetAsync("/api/Users/get-user-info");

        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_CannotSuspendSelf()
    {
        await AuthenticateAsAdminAsync(email: "admin-users-self-admin@example.com");
        var adminId = GetTokenClaim("nameid");

        var response = await Client.PutAsJsonAsync(
            $"/api/Admin/users/{adminId}/status",
            new UpdateUserStatusDto { Status = "Suspended" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<SignupResponseDto> SignUpAsync(
        string email,
        string fullName,
        string role,
        string password = "Password123")
    {
        var response = await Client.PostAsJsonAsync("/api/Accounts/sign-up", new SignUpDto
        {
            FullName = fullName,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            Role = role
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SignupResponseDto>())!;
    }
}
