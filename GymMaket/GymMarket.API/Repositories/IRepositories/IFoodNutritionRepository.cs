using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    // Data access only — business rules (caloric calculation, name uniqueness,
    // ownership checks) live in IFoodNutritionService.
    public interface IFoodNutritionRepository
    {
        // Master food database
        Task<List<FoodNutrition>> SearchFoodNutrition(string search, int skip, int take);
        Task<FoodNutrition?> GetFoodNutrition(int id);
        Task<bool> FoodNutritionNameExists(string name, int? excludeId = null);
        Task<FoodNutrition> AddFoodNutrition(FoodNutrition food);
        Task RemoveFoodNutrition(FoodNutrition food);

        // User food log
        Task<List<FoodNutritionUser>> GetFoodNutritionUser(string userId, DateOnly? date, int skip, int take);
        Task<FoodNutritionUser?> GetFoodNutritionUserEntry(int id, string userId);
        Task<FoodNutritionUser> AddFoodNutritionUserEntry(FoodNutritionUser entry);
        Task RemoveFoodNutritionUserEntry(FoodNutritionUser entry);

        // Daily nutrition targets
        Task<NutritionBudget?> GetNutritionBudget(string userId);
        Task<NutritionBudget> AddNutritionBudget(NutritionBudget budget);

        // Persists changes made to tracked entities returned by the getters above.
        Task SaveChangesAsync();
    }
}
