using System.Security.Claims;
using GymMarket.API.DTOs.FoodNutrition;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodNutritionController : ControllerBase
    {
        private readonly IFoodNutritionService _foodNutritionService;

        public FoodNutritionController(IFoodNutritionService foodNutritionService)
        {
            _foodNutritionService = foodNutritionService;
        }

        // Logs belong to the authenticated user only — never trust a client-sent id.
        private string CurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet("search-nutrition")]
        public async Task<IActionResult> SearchFoodNutrition([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _foodNutritionService.SearchFoodNutrition(search, page, pageSize);
            return Ok(list);
        }

        [HttpGet("get-nutrition-user")]
        public async Task<IActionResult> GetFoodNutritionUser([FromQuery] DateOnly? date = null, [FromQuery] int page = 1, [FromQuery] int? pageSize = null)
        {
            var list = await _foodNutritionService.GetUserLog(CurrentUserId(), date, page, pageSize);
            return Ok(list);
        }

        [HttpGet("get-nutrition-user-range")]
        public async Task<IActionResult> GetFoodNutritionUserRange([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var list = await _foodNutritionService.GetUserLogRange(CurrentUserId(), from, to);
            return Ok(list);
        }

        [HttpGet("nutrition-summary")]
        public async Task<IActionResult> GetNutritionSummary([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var summary = await _foodNutritionService.GetNutritionSummary(CurrentUserId(), from, to);
            return Ok(summary);
        }

        [HttpPost("cal-caloric-value")]
        public async Task<IActionResult> CalCaloricValue(AddFoodNutritionUser model)
        {
            var result = await _foodNutritionService.LogFood(model, CurrentUserId());
            if(result == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_NOT_FOUND" });
            }
            return Ok(result);
        }

        [HttpPost("custom-foodnutrition-user")]
        public async Task<IActionResult> CreateCustomFoodNutritionUser(AddCustomFoodNutritionUser model)
        {
            var result = await _foodNutritionService.LogCustomFood(model, CurrentUserId());
            if (result == null)
            {
                return BadRequest(new { error = "INVALID_CUSTOM_FOOD" });
            }
            return Ok(result);
        }

        [HttpPut("update-foodnutrition-user")]
        public async Task<IActionResult> UpdateFoodNutritionUser(UpdateFoodNutritionUserDto model)
        {
            var result = await _foodNutritionService.UpdateLoggedFood(model, CurrentUserId());
            if (result == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_USER_NOT_FOUND" });
            }
            return Ok(result);
        }

        [HttpDelete("delete-foodnutrition-user")]
        public async Task<IActionResult> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var deleted = await _foodNutritionService.DeleteLoggedFood(model, CurrentUserId());
            if(deleted)
            {
                return Ok(new { message = "Successfully" });
            }
            return BadRequest(new { error = "DELETE_FAILED" });
        }

        // Daily nutrition targets for the calculator's budget rings; defaults are
        // returned until the user saves their own.

        [HttpGet("nutrition-budget")]
        public async Task<IActionResult> GetNutritionBudget()
        {
            var budget = await _foodNutritionService.GetNutritionBudget(CurrentUserId());
            return Ok(budget);
        }

        [HttpPut("nutrition-budget")]
        public async Task<IActionResult> SaveNutritionBudget(NutritionBudgetDto model)
        {
            var budget = await _foodNutritionService.SaveNutritionBudget(CurrentUserId(), model);
            return Ok(budget);
        }

        // The master food database (values per 100g) is managed by admins only.
        // User logs are snapshots, so changes here never rewrite logged entries.

        [HttpPost("create-nutrition")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFoodNutrition(CreateFoodNutritionDto model)
        {
            var created = await _foodNutritionService.CreateFoodNutrition(model);
            if (created == null)
            {
                return BadRequest(new { error = "FOOD_NAME_ALREADY_EXISTS" });
            }
            return Ok(created);
        }

        [HttpPut("update-nutrition/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFoodNutrition(int id, UpdateFoodNutritionDto model)
        {
            var updated = await _foodNutritionService.UpdateFoodNutrition(id, model);
            if (updated == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_NOT_FOUND_OR_NAME_TAKEN" });
            }
            return Ok(updated);
        }

        [HttpDelete("delete-nutrition/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoodNutrition(int id)
        {
            var deleted = await _foodNutritionService.DeleteFoodNutrition(id);
            if (deleted)
            {
                return Ok(new { message = "Successfully" });
            }
            return BadRequest(new { error = "FOOD_NUTRITION_NOT_FOUND" });
        }
    }
}
