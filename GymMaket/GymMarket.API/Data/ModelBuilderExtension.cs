using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Data
{
    public static class ModelBuilderExtension
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Nutritional values are per 100g (CaloricValue in kcal, macros in grams),
            // matching the weight-based calculation in FoodNutritionRepository.
            modelBuilder.Entity<FoodNutrition>().HasData(
                new FoodNutrition { Id = 1, Name = "Apple", CaloricValue = 52, Carbs = 13.8, Fat = 0.2, Sugars = 10.4, Protein = 0.3 },
                new FoodNutrition { Id = 2, Name = "Banana", CaloricValue = 89, Carbs = 22.8, Fat = 0.3, Sugars = 12.2, Protein = 1.1 },
                new FoodNutrition { Id = 3, Name = "Orange", CaloricValue = 47, Carbs = 11.8, Fat = 0.1, Sugars = 9.4, Protein = 0.9 },
                new FoodNutrition { Id = 4, Name = "Strawberries", CaloricValue = 32, Carbs = 7.7, Fat = 0.3, Sugars = 4.9, Protein = 0.7 },
                new FoodNutrition { Id = 5, Name = "Avocado", CaloricValue = 160, Carbs = 8.5, Fat = 14.7, Sugars = 0.7, Protein = 2.0 },
                new FoodNutrition { Id = 6, Name = "Grilled Chicken Breast", CaloricValue = 165, Carbs = 0, Fat = 3.6, Sugars = 0, Protein = 31.0 },
                new FoodNutrition { Id = 7, Name = "Lean Beef Steak", CaloricValue = 250, Carbs = 0, Fat = 15.0, Sugars = 0, Protein = 26.0 },
                new FoodNutrition { Id = 8, Name = "Baked Salmon", CaloricValue = 208, Carbs = 0, Fat = 13.0, Sugars = 0, Protein = 20.0 },
                new FoodNutrition { Id = 9, Name = "Canned Tuna (in Water)", CaloricValue = 116, Carbs = 0, Fat = 0.8, Sugars = 0, Protein = 25.5 },
                new FoodNutrition { Id = 10, Name = "Cooked Shrimp", CaloricValue = 99, Carbs = 0.2, Fat = 0.3, Sugars = 0, Protein = 24.0 },
                new FoodNutrition { Id = 11, Name = "Boiled Egg", CaloricValue = 155, Carbs = 1.1, Fat = 11.0, Sugars = 1.1, Protein = 13.0 },
                new FoodNutrition { Id = 12, Name = "Plain Greek Yogurt", CaloricValue = 59, Carbs = 3.6, Fat = 0.4, Sugars = 3.2, Protein = 10.0 },
                new FoodNutrition { Id = 13, Name = "Whole Milk", CaloricValue = 61, Carbs = 4.8, Fat = 3.3, Sugars = 5.1, Protein = 3.2 },
                new FoodNutrition { Id = 14, Name = "Cheddar Cheese", CaloricValue = 403, Carbs = 1.3, Fat = 33.0, Sugars = 0.5, Protein = 25.0 },
                new FoodNutrition { Id = 15, Name = "White Rice (Cooked)", CaloricValue = 130, Carbs = 28.2, Fat = 0.3, Sugars = 0.1, Protein = 2.7 },
                new FoodNutrition { Id = 16, Name = "Brown Rice (Cooked)", CaloricValue = 111, Carbs = 23.0, Fat = 0.9, Sugars = 0.4, Protein = 2.6 },
                new FoodNutrition { Id = 17, Name = "Oatmeal (Cooked)", CaloricValue = 71, Carbs = 12.0, Fat = 1.5, Sugars = 0.3, Protein = 2.5 },
                new FoodNutrition { Id = 18, Name = "Whole Wheat Bread", CaloricValue = 247, Carbs = 41.0, Fat = 3.4, Sugars = 6.0, Protein = 13.0 },
                new FoodNutrition { Id = 19, Name = "Pasta (Cooked)", CaloricValue = 131, Carbs = 25.0, Fat = 1.1, Sugars = 0.6, Protein = 5.0 },
                new FoodNutrition { Id = 20, Name = "Baked Sweet Potato", CaloricValue = 90, Carbs = 20.7, Fat = 0.2, Sugars = 6.5, Protein = 2.0 },
                new FoodNutrition { Id = 21, Name = "Boiled Potato", CaloricValue = 87, Carbs = 20.1, Fat = 0.1, Sugars = 0.9, Protein = 1.9 },
                new FoodNutrition { Id = 22, Name = "Quinoa (Cooked)", CaloricValue = 120, Carbs = 21.3, Fat = 1.9, Sugars = 0.9, Protein = 4.4 },
                new FoodNutrition { Id = 23, Name = "Broccoli", CaloricValue = 34, Carbs = 6.6, Fat = 0.4, Sugars = 1.7, Protein = 2.8 },
                new FoodNutrition { Id = 24, Name = "Spinach", CaloricValue = 23, Carbs = 3.6, Fat = 0.4, Sugars = 0.4, Protein = 2.9 },
                new FoodNutrition { Id = 25, Name = "Carrot", CaloricValue = 41, Carbs = 9.6, Fat = 0.2, Sugars = 4.7, Protein = 0.9 },
                new FoodNutrition { Id = 26, Name = "Almonds", CaloricValue = 579, Carbs = 21.6, Fat = 49.9, Sugars = 4.4, Protein = 21.2 },
                new FoodNutrition { Id = 27, Name = "Peanut Butter", CaloricValue = 588, Carbs = 20.0, Fat = 50.0, Sugars = 9.2, Protein = 25.0 },
                new FoodNutrition { Id = 28, Name = "Lentils (Cooked)", CaloricValue = 116, Carbs = 20.1, Fat = 0.4, Sugars = 1.8, Protein = 9.0 },
                new FoodNutrition { Id = 29, Name = "Black Beans (Cooked)", CaloricValue = 132, Carbs = 23.7, Fat = 0.5, Sugars = 0.3, Protein = 8.9 },
                new FoodNutrition { Id = 30, Name = "Tofu", CaloricValue = 76, Carbs = 1.9, Fat = 4.8, Sugars = 0.6, Protein = 8.0 },
                new FoodNutrition { Id = 31, Name = "Whey Protein Powder", CaloricValue = 412, Carbs = 7.0, Fat = 7.0, Sugars = 6.0, Protein = 71.0 },
                new FoodNutrition { Id = 32, Name = "Dark Chocolate (70%)", CaloricValue = 546, Carbs = 61.0, Fat = 31.0, Sugars = 24.0, Protein = 7.9 }
            );
        }
    }
}
