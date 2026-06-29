namespace GymMarket.API.Models;

public static class NotificationDeliveryChannels
{
    public const string InApp = "in_app";
    public const string Email = "email";

    public static readonly IReadOnlyList<string> All = [InApp, Email];

    public static bool IsSupported(string? channel)
    {
        return !string.IsNullOrWhiteSpace(channel)
            && All.Any(value => string.Equals(value, channel.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static string Normalize(string channel)
    {
        return All.First(value => string.Equals(value, channel.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}

public static class NotificationDeliveryStatuses
{
    public const string Sent = "sent";
    public const string Skipped = "skipped";
    public const string Failed = "failed";

    public static readonly IReadOnlyList<string> All = [Sent, Skipped, Failed];

    public static bool IsSupported(string? status)
    {
        return !string.IsNullOrWhiteSpace(status)
            && All.Any(value => string.Equals(value, status.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static string Normalize(string status)
    {
        return All.First(value => string.Equals(value, status.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
