using GymMarket.API.Data;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class FoodNutritionRepository
    {

        private readonly GymMarketContext _context;

        public FoodNutritionRepository(GymMarketContext context)
        {
            _context = context;
        }

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string search)
        {
            var list = await _context.FoodNutritions.AsNoTrackingWithIdentityResolution()
                .Where(f => f.Name!.ToLower().Contains(search.ToLower()))
                .ToListAsync();

            return list;
        }

        public async Task<FoodNutritionUser?> CalculateCaloric(AddFoodNutritionUser model)
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
                UserId = model.UserId,
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

        public async Task<bool> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var nutrition = await _context.FoodNutritionUsers
                .AsNoTrackingWithIdentityResolution()
                .Where(f => f.UserId == model.UserId && f.Id == model.FoodNutritionUserId)
                .FirstOrDefaultAsync();

            if(nutrition == null)
            {
                return false;
            }

            _context.FoodNutritionUsers.Remove(nutrition);
            var r = await _context.SaveChangesAsync();
            return r > 0;
        }

        public async Task<FoodNutritionUser?> UpdateCalculateCaloric(AddFoodNutritionUser model)
        {
            return await CalculateCaloric(model);
        }
    }
}
