using GymMarket.API.Data;
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

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string search, int skip, int take)
        {
            var list = await _context.FoodNutritions.AsNoTrackingWithIdentityResolution()
                .Where(f => f.Name!.ToLower().Contains(search.ToLower()))
                .OrderBy(f => f.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return list;
        }

        public async Task<FoodNutrition?> GetFoodNutrition(int id)
        {
            return await _context.FoodNutritions.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> FoodNutritionNameExists(string name, int? excludeId = null)
        {
            return await _context.FoodNutritions
                .AnyAsync(f => f.Name!.ToLower() == name.ToLower() && (excludeId == null || f.Id != excludeId));
        }

        public async Task<FoodNutrition> AddFoodNutrition(FoodNutrition food)
        {
            _context.FoodNutritions.Add(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task RemoveFoodNutrition(FoodNutrition food)
        {
            _context.FoodNutritions.Remove(food);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FoodNutritionUser>> GetFoodNutritionUser(string userId, DateOnly? date, int skip, int take)
        {
            return await _context.FoodNutritionUsers.AsNoTrackingWithIdentityResolution()
                .Where(f => f.UserId == userId && (date == null || f.Date == date))
                .OrderBy(f => f.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<FoodNutritionUser?> GetFoodNutritionUserEntry(int id, string userId)
        {
            return await _context.FoodNutritionUsers
                .FirstOrDefaultAsync(f => f.UserId == userId && f.Id == id);
        }

        public async Task<FoodNutritionUser> AddFoodNutritionUserEntry(FoodNutritionUser entry)
        {
            _context.FoodNutritionUsers.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task RemoveFoodNutritionUserEntry(FoodNutritionUser entry)
        {
            _context.FoodNutritionUsers.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<NutritionBudget?> GetNutritionBudget(string userId)
        {
            return await _context.NutritionBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<NutritionBudget> AddNutritionBudget(NutritionBudget budget)
        {
            _context.NutritionBudgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
