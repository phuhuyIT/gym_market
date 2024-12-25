using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseRegistration;
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
            double fat = model.Weight / 100f * foodNutrition.Fat;
            double protein = model.Weight / 100f * foodNutrition.Protein;
            double sugars = model.Weight / 100f * foodNutrition.Sugars;

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

        public async Task<bool> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var nutrition = await context.FoodNutritionUsers
                .AsNoTrackingWithIdentityResolution()
                .Where(f => f.UserId == model.UserId && f.Id == model.FoodNutritionUserId)
                .FirstOrDefaultAsync();

            if(nutrition == null)
            {
                return false;
            }

            context.FoodNutritionUsers.Remove(nutrition);
            var r = await context.SaveChangesAsync();
            if(r > 0) { return true; }
            return false;
        }

        public async Task<FoodNutritionUser?> UpdateCalculateCaloric(AddFoodNutritionUser model)
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
            double fat = model.Weight / 100f * foodNutrition.Fat;
            double protein = model.Weight / 100f * foodNutrition.Protein;
            double sugars = model.Weight / 100f * foodNutrition.Sugars;

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
            context.FoodNutritionUsers.Add(newFoodNutritionUser);
            var r = await context.SaveChangesAsync();
            if (r > 0)
            {
                return newFoodNutritionUser;
            }
            return null;
        }
    }
}
