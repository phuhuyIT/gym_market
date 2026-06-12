using GymMarket.API.Data;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class FoodNutritionRepository : IFoodNutritionRepository
    {

        private readonly GymMarketContext _context;

        public FoodNutritionRepository(GymMarketContext context)
        {
            _context = context;
        }

        private const int MaxSearchResults = 20;

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string search)
        {
            var list = await _context.FoodNutritions.AsNoTrackingWithIdentityResolution()
                .Where(f => f.Name!.ToLower().Contains(search.ToLower()))
                .OrderBy(f => f.Name)
                .Take(MaxSearchResults)
                .ToListAsync();

            return list;
        }

        public async Task<FoodNutritionUser?> CalculateCaloric(AddFoodNutritionUser model, string userId)
        {
            var foodNutrition = await _context.FoodNutritions
                .AsNoTrackingWithIdentityResolution()
                .Where(f => f.Id == model.FoodNutritionId)
                .FirstOrDefaultAsync();

            if (foodNutrition == null)
            {
                return null;
            }

            double calo = (double)model.Weight / 100.0 * foodNutrition.CaloricValue;
            double fat = (double)model.Weight / 100.0 * foodNutrition.Fat;
            double protein = (double)model.Weight / 100.0 * foodNutrition.Protein;
            double sugars = (double)model.Weight / 100.0 * foodNutrition.Sugars;

            var newFoodNutritionUser = new FoodNutritionUser()
            {
                CaloricValue = calo,
                FoodName = model.FoodName,
                Weight = model.Weight,
                Fat = fat,
                Protein = protein,
                Sugars = sugars,
                UserId = userId,
                Date = model.Date ?? DateOnly.FromDateTime(DateTime.Today),
                MealType = model.MealType,
            };
            _context.FoodNutritionUsers.Add(newFoodNutritionUser);
            var r = await _context.SaveChangesAsync();
            if (r > 0)
            {
                return newFoodNutritionUser;
            }
            return null;
        }

        public async Task<List<FoodNutritionUser>> GetFoodNutritionUser(string userId)
        {
            var list = await _context.FoodNutritionUsers.AsNoTrackingWithIdentityResolution()
              .Where(f => f.UserId == userId)
              .ToListAsync();

            return list;
        }

        public async Task<bool> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model, string userId)
        {
            var nutrition = await _context.FoodNutritionUsers
                .Where(f => f.UserId == userId && f.Id == model.FoodNutritionUserId)
                .FirstOrDefaultAsync();

            if(nutrition == null)
            {
                return false;
            }

            _context.FoodNutritionUsers.Remove(nutrition);
            var r = await _context.SaveChangesAsync();
            return r > 0;
        }

        public async Task<FoodNutritionUser?> UpdateFoodNutritionUser(UpdateFoodNutritionUserDto model, string userId)
        {
            var nutrition = await _context.FoodNutritionUsers
                .Where(f => f.UserId == userId && f.Id == model.FoodNutritionUserId)
                .FirstOrDefaultAsync();

            if (nutrition == null || nutrition.Weight <= 0)
            {
                return null;
            }

            // The stored macros scale linearly with weight, so rescale them instead of
            // re-reading FoodNutritions (the log doesn't keep the source food's id).
            var factor = model.Weight / nutrition.Weight;
            nutrition.CaloricValue *= factor;
            nutrition.Fat *= factor;
            nutrition.Sugars *= factor;
            nutrition.Protein *= factor;
            nutrition.Weight = model.Weight;

            if (model.Date.HasValue)
            {
                nutrition.Date = model.Date;
            }
            if (!string.IsNullOrWhiteSpace(model.MealType))
            {
                nutrition.MealType = model.MealType;
            }

            await _context.SaveChangesAsync();
            return nutrition;
        }
    }
}
