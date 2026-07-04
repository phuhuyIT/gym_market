namespace GymMarket.API.Models;

public static class CourseLiveSessionStatus
{
    public const string Draft = "Draft";
    public const string Scheduled = "Scheduled";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All =
    [
        Draft,
        Scheduled,
        Completed,
        Cancelled,
    ];

    public static string Normalize(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return Draft;

        var match = All.FirstOrDefault(value => string.Equals(value, status.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? Draft;
    }

    public static bool IsLearnerVisible(string? status)
    {
        var normalized = Normalize(status);
        return normalized is Scheduled or Completed or Cancelled;
    }
}
