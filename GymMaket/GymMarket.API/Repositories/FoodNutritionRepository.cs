using GymMarket.API.Data;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class FoodNutritionRepository
    {

        private readonly GymMarketContext context;

        public FoodNutritionRepository(GymMarketContext context)
        {
            this.context = context;
        }

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string search)
        {
            var list = await context.FoodNutritions.AsNoTrackingWithIdentityResolution()
                .Where(f => f.Name.ToLower().Contains(search.ToLower()))
                .ToListAsync();

            return list;
        }

        public async Task<FoodNutritionUser?> CalculateCaloric(AddFoodNutritionUser model)
        {
            var foodNutrition = await context.FoodNutritions
                .AsNoTrackingWithIdentityResolution()
                .Where(f => f.Id == model.FoodNutritionId)
                .FirstOrDefaultAsync();

            if (foodNutrition == null)
            {
                return null;
            }

            double calo = model.Weight / 100f * foodNutrition.CaloricValue;

            var newFoodNutritionUser = new FoodNutritionUser()
            {
                CaloricValue = calo,
                FoodName = model.FoodName,
                Weight = model.Weight,
                UserId = model.UserId,
            };
            context.FoodNutritionUsers.Add(newFoodNutritionUser);
            var r = await context.SaveChangesAsync();
            if (r > 0)
            {
                return newFoodNutritionUser;
            }
            return null;
        }

        public async Task<List<FoodNutritionUser>> GetFoodNutritionUser(string userId)
        {
            var list = await context.FoodNutritionUsers.AsNoTrackingWithIdentityResolution()
              .Where(f => f.UserId == userId)
              .ToListAsync();

            return list;
        }
    }
}
