using AutoMapper;
using GymMarket.API.DTOs.FoodNutrition;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;

namespace GymMarket.API.Services
{
    public class FoodNutritionService : IFoodNutritionService
    {
        private const int DefaultSearchPageSize = 20;
        private const int MaxPageSize = 200;

        // Defaults shown by the calculator UI when a user has not saved targets yet.
        private const double DefaultCalorieBudget = 2000;
        private const double DefaultCarbsBudget = 250;
        private const double DefaultFatBudget = 65;
        private const double DefaultProteinBudget = 130;

        private readonly IFoodNutritionRepository _repository;
        private readonly IMapper _mapper;

        public FoodNutritionService(IFoodNutritionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<FoodNutrition>> SearchFoodNutrition(string search, int page, int pageSize)
        {
            var (skip, take) = Page(page, pageSize, DefaultSearchPageSize);
            return await _repository.SearchFoodNutrition(search, skip, take);
        }

        public async Task<FoodNutritionUser?> LogFood(AddFoodNutritionUser model, string userId)
        {
            var food = await _repository.GetFoodNutrition(model.FoodNutritionId);
            if (food == null)
            {
                return null;
            }

            var entry = _mapper.Map<FoodNutritionUser>(model);
            entry.UserId = userId;
            entry.Date ??= DateOnly.FromDateTime(DateTime.Today);
            ApplyNutrition(entry, food, model.Weight);

            return await _repository.AddFoodNutritionUserEntry(entry);
        }

        public async Task<List<FoodNutritionUser>> GetUserLog(string userId, DateOnly? date, int page, int? pageSize)
        {
            // No pageSize means the caller wants the full log (the calculator's
            // weekly view filters client-side).
            if (pageSize == null)
            {
                return await _repository.GetFoodNutritionUser(userId, date, 0, int.MaxValue);
            }

            var (skip, take) = Page(page, pageSize.Value, DefaultSearchPageSize);
            return await _repository.GetFoodNutritionUser(userId, date, skip, take);
        }

        public async Task<FoodNutritionUser?> UpdateLoggedFood(UpdateFoodNutritionUserDto model, string userId)
        {
            var entry = await _repository.GetFoodNutritionUserEntry(model.FoodNutritionUserId, userId);
            if (entry == null)
            {
                return null;
            }

            var food = entry.FoodNutritionId == null
                ? null
                : await _repository.GetFoodNutrition(entry.FoodNutritionId.Value);

            if (food != null)
            {
                ApplyNutrition(entry, food, model.Weight);
            }
            else if (entry.Weight > 0)
            {
                // Legacy rows have no source food id; their stored macros scale
                // linearly with weight, so rescale the snapshot instead.
                var factor = model.Weight / entry.Weight;
                entry.CaloricValue *= factor;
                entry.Fat *= factor;
                entry.Sugars *= factor;
                entry.Protein *= factor;
                entry.Weight = model.Weight;
            }
            else
            {
                return null;
            }

            if (model.Date.HasValue)
            {
                entry.Date = model.Date;
            }
            if (!string.IsNullOrWhiteSpace(model.MealType))
            {
                entry.MealType = model.MealType;
            }

            await _repository.SaveChangesAsync();
            return entry;
        }

        public async Task<bool> DeleteLoggedFood(DeleteFoodNutritionUserDto model, string userId)
        {
            var entry = await _repository.GetFoodNutritionUserEntry(model.FoodNutritionUserId, userId);
            if (entry == null)
            {
                return false;
            }

            await _repository.RemoveFoodNutritionUserEntry(entry);
            return true;
        }

        public async Task<FoodNutrition?> CreateFoodNutrition(CreateFoodNutritionDto model)
        {
            var name = model.Name.Trim();
            if (await _repository.FoodNutritionNameExists(name))
            {
                return null;
            }

            var food = _mapper.Map<FoodNutrition>(model);
            food.Name = name;
            return await _repository.AddFoodNutrition(food);
        }

        public async Task<FoodNutrition?> UpdateFoodNutrition(int id, UpdateFoodNutritionDto model)
        {
            var food = await _repository.GetFoodNutrition(id);
            if (food == null)
            {
                return null;
            }

            var name = model.Name.Trim();
            if (await _repository.FoodNutritionNameExists(name, excludeId: id))
            {
                return null;
            }

            // User logs are snapshots of the values at logging time and are
            // intentionally left untouched here.
            _mapper.Map(model, food);
            food.Name = name;
            await _repository.SaveChangesAsync();
            return food;
        }

        public async Task<bool> DeleteFoodNutrition(int id)
        {
            var food = await _repository.GetFoodNutrition(id);
            if (food == null)
            {
                return false;
            }

            await _repository.RemoveFoodNutrition(food);
            return true;
        }

        public async Task<NutritionBudgetDto> GetNutritionBudget(string userId)
        {
            var budget = await _repository.GetNutritionBudget(userId);
            if (budget == null)
            {
                return new NutritionBudgetDto
                {
                    CalorieBudget = DefaultCalorieBudget,
                    CarbsBudget = DefaultCarbsBudget,
                    FatBudget = DefaultFatBudget,
                    ProteinBudget = DefaultProteinBudget,
                };
            }

            return _mapper.Map<NutritionBudgetDto>(budget);
        }

        public async Task<NutritionBudgetDto> SaveNutritionBudget(string userId, NutritionBudgetDto model)
        {
            var budget = await _repository.GetNutritionBudget(userId);
            if (budget == null)
            {
                budget = _mapper.Map<NutritionBudget>(model);
                budget.UserId = userId;
                await _repository.AddNutritionBudget(budget);
            }
            else
            {
                _mapper.Map(model, budget);
                await _repository.SaveChangesAsync();
            }

            return _mapper.Map<NutritionBudgetDto>(budget);
        }

        // Nutritional values in the master database are per 100g.
        private static void ApplyNutrition(FoodNutritionUser entry, FoodNutrition food, double weight)
        {
            entry.Weight = weight;
            entry.CaloricValue = weight / 100.0 * food.CaloricValue;
            entry.Fat = weight / 100.0 * food.Fat;
            entry.Sugars = weight / 100.0 * food.Sugars;
            entry.Protein = weight / 100.0 * food.Protein;
        }

        private static (int skip, int take) Page(int page, int pageSize, int defaultPageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = defaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
            return ((page - 1) * pageSize, pageSize);
        }
    }
}
