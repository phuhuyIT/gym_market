namespace GymMarket.API.Models
{
    public static class NotificationTypes
    {
        public const string System = "system";
        public const string Payment = "payment";
        public const string Chat = "chat";
        public const string Course = "course";
        public const string Assignment = "assignment";
        public const string Quiz = "quiz";
        public const string Announcement = "announcement";
        public const string LiveSession = "live_session";
        public const string Discussion = "discussion";
        public const string StudyGroup = "study_group";
        public const string Grading = "grading";
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
            Assignment,
            Quiz,
            Announcement,
            LiveSession,
            Discussion,
            StudyGroup,
            Grading,
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
            [Assignment] = "Assignment",
            [Quiz] = "Quiz",
            [Announcement] = "Course announcement",
            [LiveSession] = "Live session",
            [Discussion] = "Course discussion",
            [StudyGroup] = "Study group",
            [Grading] = "Grading",
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

    public static class NotificationEmailFrequencies
    {
        public const string Immediate = "immediate";
        public const string Daily = "daily";
        public const string Weekly = "weekly";
        public const string Off = "off";

        public static readonly IReadOnlyList<string> All =
        [
            Immediate,
            Daily,
            Weekly,
            Off,
        ];

        public static bool IsSupported(string? frequency)
        {
            return !string.IsNullOrWhiteSpace(frequency)
                && All.Contains(frequency.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        public static string Normalize(string frequency)
        {
            return frequency.Trim().ToLowerInvariant();
        }
    }

}
