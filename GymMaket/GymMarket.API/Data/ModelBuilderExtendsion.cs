using GymMarket.API.Models;
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
            var role3 = new IdentityRole() { Id = "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", Name = "Member", ConcurrencyStamp = "3", NormalizedName = "Member" };

            modelBuilder.Entity<IdentityRole>()
                .HasData(role1, role2, role3);

    
            var trainer1 = new Trainer
            {
                TrainerId = "TR001",
                Name = "John Doe",
                Certification = "YogaStrong",
                Rating = 4.5m
            };

            var trainer2 = new Trainer
            {
                TrainerId = "TR002",
                Name = "Jane Smith",
              Certification="GymStrong",
                Rating = 4.8m
            };

            modelBuilder.Entity<Trainer>()
                .HasData(trainer1, trainer2);

 
            var course1 = new Course
            {
                CourseId = "C001",
                Title = "Beginner Yoga",
                Description = "A beginner-level yoga course.",
                TrainerId = trainer1.TrainerId,
                Category = "Yoga",
                Price = 100m,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(40),
                Duration = 30,
                MaxParticipants = 20,
                Rating = 4.6m
            };

            var course2 = new Course
            {
                CourseId = "C002",
                Title = "Advanced Fitness",
                Description = "An advanced course for fitness enthusiasts.",
                TrainerId = trainer2.TrainerId,
                Category = "Fitness",
                Price = 150m,
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(45),
                Duration = 30,
                MaxParticipants = 25,
                Rating = 4.9m
            };

            modelBuilder.Entity<Course>()
                .HasData(course1, course2);
        }
    }
}
