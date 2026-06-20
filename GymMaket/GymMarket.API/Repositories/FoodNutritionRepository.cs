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

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string? search, int skip, int take)
        {
            var normalizedSearch = search?.Trim();
            var query = _context.FoodNutritions.AsNoTrackingWithIdentityResolution();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(f => f.Name != null && f.Name.Contains(normalizedSearch));
            }

            var list = await query
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
            name = name.Trim();
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                var normalizedName = name.ToLower();
                return await _context.FoodNutritions
                    .AnyAsync(f => f.Name != null && f.Name.ToLower() == normalizedName && (excludeId == null || f.Id != excludeId));
            }

            return await _context.FoodNutritions
                .AnyAsync(f => f.Name == name && (excludeId == null || f.Id != excludeId));
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

        public async Task<List<FoodNutritionUser>> GetFoodNutritionUserRange(string userId, DateOnly from, DateOnly to)
        {
            return await _context.FoodNutritionUsers.AsNoTrackingWithIdentityResolution()
                .Where(f => f.UserId == userId && f.Date >= from && f.Date <= to)
                .OrderBy(f => f.Date)
                .ThenBy(f => f.Id)
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
