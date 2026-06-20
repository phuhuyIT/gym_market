namespace GymMarket.API
{
    public static class PaymentStatus
    {
        public const string Completed = "COMPLETED";
        public const string Paid = "Paid";
        public const string Pending = "Pending";
        public const string PendingPayment = "Pending Payment";
        public const string NotStarted = "Not Started";
        public const string Canceled = "Canceled";
        public const string Expired = "Expired";

        // The gateway flow historically wrote successful payments as "COMPLETED" while
        // the manual-approval flow writes "Paid". Both mean the same thing, but the client
        // only understands Pending/Paid/Canceled. New writes always use Paid; this collapses
        // any legacy "COMPLETED" rows so neither read path mislabels a paid student.
        public static string? Normalize(string? status) =>
            status == Completed ? Paid : status;

        // True when the status represents a successfully paid course.
        public static bool IsPaid(string? status) =>
            status == Paid || status == Completed;
    }

    public static class PaymentType
    {
        public const string Momo = "MOMO";
    }

    public static class FileType
    {
        public const string Image = "IMAGE";
        public const string Video = "VIDEO";
    }

    public static class Defaults
    {
        public const string AvatarUrl = "https://cdn-icons-png.flaticon.com/512/1999/1999625.png";
        public const string StudentAvatarUrl = "https://cdn-icons-png.flaticon.com/512/236/236832.png";
        public const int PageSize = 15;
        public const int MomoSuccessResultCode = 0;
        public const int PaymentTimeoutMinutes = 5;
        public const int DefaultRating = 0;
    }
}
