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

        public static bool IsOpen(string? status)
        {
            status = Normalize(status);
            return status == Pending || status == NotStarted || status == PendingPayment;
        }

        public static string[] OpenStatuses() =>
        [
            Pending,
            NotStarted,
            PendingPayment
        ];
    }

    public static class PaymentType
    {
        public const string Momo = "MOMO";
        public const string BankTransfer = "BANK_TRANSFER";
    }

    public static class CourseStatus
    {
        public const string Draft = "Draft";
        public const string Published = "Published";
        public const string Archived = "Archived";

        public static string Normalize(string? status) =>
            string.IsNullOrWhiteSpace(status) ? Published : status.Trim();
    }

    public static class CourseRegistrationErrorCode
    {
        public const string CourseNotFound = "COURSE_NOT_FOUND";
        public const string CourseNotPublished = "COURSE_NOT_PUBLISHED";
        public const string CourseFull = "COURSE_FULL";
        public const string InvalidCourseOption = "INVALID_COURSE_OPTION";
        public const string RegistrationNotFound = "REGISTRATION_NOT_FOUND";
        public const string RegistrationCanceled = "REGISTRATION_CANCELED";
        public const string MomoNotConfigured = "MOMO_NOT_CONFIGURED";
        public const string MomoProviderUnavailable = "MOMO_PROVIDER_UNAVAILABLE";
    }

    public static class PaymentErrorCode
    {
        public const string PaymentNotFound = "PAYMENT_NOT_FOUND";
        public const string PaymentAlreadyFinalized = "PAYMENT_ALREADY_FINALIZED";
        public const string PaymentObsolete = "PAYMENT_OBSOLETE";
        public const string GatewayPaymentManualApprovalNotAllowed = "GATEWAY_PAYMENT_MANUAL_APPROVAL_NOT_ALLOWED";
    }

    public static class PaymentEventType
    {
        public const string Created = "Created";
        public const string MomoCreated = "MomoCreated";
        public const string MomoReturned = "MomoReturned";
        public const string MomoIpnReceived = "MomoIpnReceived";
        public const string Paid = "Paid";
        public const string Canceled = "Canceled";
        public const string Expired = "Expired";
        public const string ManualApproved = "ManualApproved";
        public const string ManualCanceled = "ManualCanceled";
        public const string ReplacedBySuccessfulPayment = "ReplacedBySuccessfulPayment";
    }

    public static class PaymentEventSource
    {
        public const string System = "System";
        public const string Student = "Student";
        public const string Trainer = "Trainer";
        public const string Momo = "Momo";
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
