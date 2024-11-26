using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Data
{
    public static class ModelBuilderExtendsion
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // roles
            var role1 = new IdentityRole() { Id = "25501994-44dd-44b8-bb7d-1b2af376f1be", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" };
            var role2 = new IdentityRole() { Id = "32b89678-1f5d-43c8-8dbd-4251902bdfa4", Name = "Trainer", ConcurrencyStamp = "2", NormalizedName = "Trainer" };
            var role3 = new IdentityRole() { Id = "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", Name = "Student", ConcurrencyStamp = "3", NormalizedName = "Student" };

            modelBuilder.Entity<IdentityRole>()
                .HasData(role1, role2, role3);
        }
    }
}
