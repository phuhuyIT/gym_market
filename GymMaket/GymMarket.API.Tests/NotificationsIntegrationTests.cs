using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class NotificationsIntegrationTests : BaseIntegrationTests
{
    public NotificationsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task MarkTypeAsRead_OnlyClearsNotificationsOfSelectedType()
    {
        const string email = "notification-type-student@example.com";
        await AuthenticateAsync(email: email, role: "Student");
        await SeedNotification(email, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(email, NotificationTypes.Class, "Class cancelled", "/client/classes");
        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        var response = await Client.PostAsync($"/api/Notifications/mark-type-read/{NotificationTypes.Class}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.NotNull(notifications);
        Assert.All(notifications!.Where(n => n.Type == NotificationTypes.Class), n => Assert.True(n.IsRead));
        Assert.Contains(notifications, n => n.Type == NotificationTypes.Workout && !n.IsRead);
    }

    [Fact]
    public async Task GetNotifications_CanFilterByTypeAndReadStatus()
    {
        const string email = "notification-filter-student@example.com";
        await AuthenticateAsync(email: email, role: "Student");
        await SeedNotification(email, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(email, NotificationTypes.Class, "Class cancelled", "/client/classes");
        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");
        await Client.PostAsync($"/api/Notifications/mark-type-read/{NotificationTypes.Class}", null);

        var classRead = await Client.GetFromJsonAsync<List<NotificationDto>>(
            $"/api/Notifications/get-notifications?type={NotificationTypes.Class}&isRead=true&take=10");
        var unread = await Client.GetFromJsonAsync<List<NotificationDto>>(
            "/api/Notifications/get-notifications?isRead=false&take=10");

        Assert.NotNull(classRead);
        Assert.Equal(2, classRead!.Count);
        Assert.All(classRead, n =>
        {
            Assert.Equal(NotificationTypes.Class, n.Type);
            Assert.True(n.IsRead);
        });
        Assert.NotNull(unread);
        Assert.Single(unread!);
        Assert.Equal(NotificationTypes.Workout, unread![0].Type);
        Assert.False(unread[0].IsRead);
    }

    private async Task SeedNotification(string email, string type, string title, string link)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var user = await userManager.FindByEmailAsync(email);

        await notificationRepository.NotifyUser(
            user!.Id,
            type,
            title,
            $"{title} content",
            link);
    }
}
