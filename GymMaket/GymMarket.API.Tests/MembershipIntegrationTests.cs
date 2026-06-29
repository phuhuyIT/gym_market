using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Membership;
using GymMarket.API.DTOs.Notifications;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class MembershipIntegrationTests : BaseIntegrationTests
{
    public MembershipIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePlan_AsStudent_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "membership-plan-student@example.com", role: "Student");

        var response = await Client.PostAsJsonAsync("/api/Memberships/plans", new UpsertMembershipPlanDto
        {
            Name = "Monthly",
            DurationDays = 30,
            Price = 49
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Subscribe_AsStudent_ActivatesMembership()
    {
        var plan = await CreatePlan("Monthly Access", 30, 49);
        await AuthenticateAsync(email: "membership-subscribe-student@example.com", role: "Student");

        var response = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var membership = await response.Content.ReadFromJsonAsync<StudentMembershipDto>();
        Assert.NotNull(membership);
        Assert.Equal(plan.PlanId, membership!.PlanId);
        Assert.Equal("Active", membership.Status);
        Assert.True(membership.EndsAt > membership.StartsAt);

        var status = await Client.GetFromJsonAsync<MembershipStatusDto>("/api/Memberships/me/status");
        Assert.NotNull(status);
        Assert.True(status!.HasActiveMembership);
        Assert.Equal(membership.MembershipId, status.CurrentMembership!.MembershipId);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == "membership"
            && n.Title == "Membership active"
            && n.Link == "/client/membership"
            && n.Content!.Contains(plan.Name));
    }

    [Fact]
    public async Task GetMyStatus_WhenMembershipExpiresSoon_AddsReminderNotification()
    {
        var plan = await CreatePlan("Seven Day Access", 7, 19);
        await AuthenticateAsync(email: "membership-expiring-student@example.com", role: "Student");
        var subscribeResponse = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });
        subscribeResponse.EnsureSuccessStatusCode();

        var status = await Client.GetFromJsonAsync<MembershipStatusDto>("/api/Memberships/me/status");

        Assert.NotNull(status);
        Assert.True(status!.HasActiveMembership);
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == "membership"
            && n.Title == "Membership expiring soon"
            && n.Link == "/client/membership");
    }

    [Fact]
    public async Task Subscribe_WithExistingActiveMembership_ExtendsFromCurrentEnd()
    {
        var plan = await CreatePlan("Renewal Plan", 14, 20);
        await AuthenticateAsync(email: "membership-renew-student@example.com", role: "Student");

        var firstResponse = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });
        firstResponse.EnsureSuccessStatusCode();
        var first = (await firstResponse.Content.ReadFromJsonAsync<StudentMembershipDto>())!;

        var secondResponse = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });
        secondResponse.EnsureSuccessStatusCode();
        var second = (await secondResponse.Content.ReadFromJsonAsync<StudentMembershipDto>())!;

        Assert.True(second.StartsAt >= first.EndsAt);
        Assert.True(second.EndsAt > first.EndsAt);
    }

    [Fact]
    public async Task CancelMine_WithActiveMembership_CancelsMembership()
    {
        var plan = await CreatePlan("Cancel Plan", 30, 30);
        await AuthenticateAsync(email: "membership-cancel-student@example.com", role: "Student");
        var subscribeResponse = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });
        subscribeResponse.EnsureSuccessStatusCode();

        var cancelResponse = await Client.PostAsync("/api/Memberships/me/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        var status = await Client.GetFromJsonAsync<MembershipStatusDto>("/api/Memberships/me/status");
        Assert.NotNull(status);
        Assert.False(status!.HasActiveMembership);
    }

    [Fact]
    public async Task GetSubscriptions_AsAdmin_ReturnsStudentMemberships()
    {
        var plan = await CreatePlan("Admin List Plan", 60, 80);
        await AuthenticateAsync(email: "membership-list-student@example.com", role: "Student");
        var subscribeResponse = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = plan.PlanId
        });
        subscribeResponse.EnsureSuccessStatusCode();

        await AuthenticateAsAdminAsync(email: "membership-list-admin@example.com");
        var subscriptions = await Client.GetFromJsonAsync<List<StudentMembershipDto>>("/api/Memberships/subscriptions?status=Active");

        Assert.NotNull(subscriptions);
        Assert.Contains(subscriptions!, s => s.PlanId == plan.PlanId && s.Status == "Active");
    }

    private async Task<MembershipPlanDto> CreatePlan(string name, int durationDays, decimal price)
    {
        await AuthenticateAsAdminAsync(email: $"membership-admin-{Guid.NewGuid():N}@example.com");
        var response = await Client.PostAsJsonAsync("/api/Memberships/plans", new UpsertMembershipPlanDto
        {
            Name = name,
            Description = $"{durationDays}-day access",
            DurationDays = durationDays,
            Price = price,
            IsActive = true
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MembershipPlanDto>())!;
    }
}
