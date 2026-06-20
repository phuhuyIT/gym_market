using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class UpdateFoodNutritionUserDto
    {
        [Required(ErrorMessage = "FoodNutritionUserId is required.")]
        public int FoodNutritionUserId { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public double Weight { get; set; }

        // Optional custom-food snapshot edits. Database-backed entries still
        // recalculate from the master food record when weight changes.
        [Range(0, double.MaxValue, ErrorMessage = "CaloricValue must not be negative.")]
        public double? CaloricValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Carbs must not be negative.")]
        public double? Carbs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fat must not be negative.")]
        public double? Fat { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Sugars must not be negative.")]
        public double? Sugars { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Protein must not be negative.")]
        public double? Protein { get; set; }

        public DateOnly? Date { get; set; }
        public string? MealType { get; set; }
    }
}
