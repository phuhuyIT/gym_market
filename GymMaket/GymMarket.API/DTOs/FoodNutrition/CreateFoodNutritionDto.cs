using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutrition
{
    // Nutritional values are per 100g, matching the weight-based
    // calculation in FoodNutritionRepository.
    public class CreateFoodNutritionDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, long.MaxValue, ErrorMessage = "CaloricValue must not be negative.")]
        public long CaloricValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fat must not be negative.")]
        public double Fat { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Sugars must not be negative.")]
        public double Sugars { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Protein must not be negative.")]
        public double Protein { get; set; }
    }
}
