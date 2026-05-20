using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Services
{
    public static class IdentityRoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var rolesWithIds = new Dictionary<string, string>
            {
                { "Admin", "25501994-44dd-44b8-bb7d-1b2af376f1be" },
                { ApplicationRoles.Trainer, "32b89678-1f5d-43c8-8dbd-4251902bdfa4" },
                { ApplicationRoles.Student, "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7" }
            };

            foreach (var kvp in rolesWithIds)
            {
                var roleName = kvp.Key;
                var roleId = kvp.Value;

                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new IdentityRole
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Failed to seed role '{roleName}': {errors}");
                }
            }
        }
    }
}
