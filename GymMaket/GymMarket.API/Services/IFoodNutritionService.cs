using GymMarket.API.DTOs.FoodNutrition;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;

namespace GymMarket.API.Services
{
    public interface IFoodNutritionService
    {
        Task<List<FoodNutrition>> SearchFoodNutrition(string search, int page, int pageSize);

        // User food log
        Task<FoodNutritionUser?> LogFood(AddFoodNutritionUser model, string userId);
        Task<List<FoodNutritionUser>> GetUserLog(string userId, DateOnly? date, int page, int? pageSize);
        Task<FoodNutritionUser?> UpdateLoggedFood(UpdateFoodNutritionUserDto model, string userId);
        Task<bool> DeleteLoggedFood(DeleteFoodNutritionUserDto model, string userId);

        // Admin management of the master food database
        Task<FoodNutrition?> CreateFoodNutrition(CreateFoodNutritionDto model);
        Task<FoodNutrition?> UpdateFoodNutrition(int id, UpdateFoodNutritionDto model);
        Task<bool> DeleteFoodNutrition(int id);

        // Daily nutrition targets
        Task<NutritionBudgetDto> GetNutritionBudget(string userId);
        Task<NutritionBudgetDto> SaveNutritionBudget(string userId, NutritionBudgetDto model);
    }
}
