using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class AddFoodNutritionUser
    {
        [Required(ErrorMessage = "FoodNutritionId is required.")]
        public int FoodNutritionId { get; set; }

        [Required(ErrorMessage = "FoodName is required.")]
        public string FoodName { get; set; } = string.Empty;

        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public double Weight { get; set; }

        public DateOnly? Date { get; set; }
        public string? MealType { get; set; }
    }
}
