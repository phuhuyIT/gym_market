using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class UpdateFoodNutritionUserDto
    {
        [Required(ErrorMessage = "FoodNutritionUserId is required.")]
        public int FoodNutritionUserId { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public double Weight { get; set; }

        public DateOnly? Date { get; set; }
        public string? MealType { get; set; }
    }
}
