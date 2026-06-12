using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IFoodNutritionRepository
    {
        Task<List<FoodNutrition>> SearchFoodNutrition(string search);
        Task<FoodNutritionUser?> CalculateCaloric(AddFoodNutritionUser model, string userId);
        Task<List<FoodNutritionUser>> GetFoodNutritionUser(string userId);
        Task<bool> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model, string userId);
        Task<FoodNutritionUser?> UpdateFoodNutritionUser(UpdateFoodNutritionUserDto model, string userId);
    }
}
