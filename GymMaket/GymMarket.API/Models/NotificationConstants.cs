namespace GymMarket.API.Models
{
    public static class NotificationTypes
    {
        public const string System = "system";
        public const string Payment = "payment";
        public const string Chat = "chat";
        public const string Course = "course";
        public const string Announcement = "announcement";
        public const string Class = "class";
        public const string Workout = "workout";
        public const string Progress = "progress";
        public const string Membership = "membership";

        public static readonly IReadOnlyList<string> All =
        [
            System,
            Payment,
            Chat,
            Course,
            Announcement,
            Class,
            Workout,
            Progress,
            Membership,
        ];

        private static readonly IReadOnlyDictionary<string, string> Labels = new Dictionary<string, string>
        {
            [System] = "System",
            [Payment] = "Payment",
            [Chat] = "Chat",
            [Course] = "Course",
            [Announcement] = "Course announcement",
            [Class] = "Class",
            [Workout] = "Workout",
            [Progress] = "Progress",
            [Membership] = "Membership",
        };

        public static bool IsSupported(string? type)
        {
            return !string.IsNullOrWhiteSpace(type)
                && All.Contains(type.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        public static string Normalize(string type)
        {
            return type.Trim().ToLowerInvariant();
        }

        public static string LabelFor(string type)
        {
            return Labels.TryGetValue(Normalize(type), out var label) ? label : type;
        }
    }
}
