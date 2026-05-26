using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class DeleteFoodNutritionUserDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "FoodNutritionUserId is required.")]
        public int FoodNutritionUserId { get; set; }
    }
}
