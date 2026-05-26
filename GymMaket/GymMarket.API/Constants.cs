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
