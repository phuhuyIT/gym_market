namespace GymMarket.API.Services
{
    public static class ApplicationRoles
    {
        public const string Trainer = "Trainer";
        public const string Student = "Student";

        public static readonly string[] All = [Trainer, Student];

        public static bool TryNormalize(string? role, out string normalizedRole)
        {
            normalizedRole = string.Empty;

            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            var match = All.FirstOrDefault(
                supportedRole => string.Equals(supportedRole, role.Trim(), StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                return false;
            }

            normalizedRole = match;
            return true;
        }
    }
}
