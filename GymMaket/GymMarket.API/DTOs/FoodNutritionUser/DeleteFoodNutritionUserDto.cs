using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class DeleteFoodNutritionUserDto
    {
        [Required(ErrorMessage = "FoodNutritionUserId is required.")]
        public int FoodNutritionUserId { get; set; }
    }
}
