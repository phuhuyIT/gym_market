using System.Security.Claims;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodNutritionController : ControllerBase
    {
        private readonly IFoodNutritionRepository _foodNutritionRepository;

        public FoodNutritionController(IFoodNutritionRepository foodNutritionRepository)
        {
            _foodNutritionRepository = foodNutritionRepository;
        }

        // Logs belong to the authenticated user only — never trust a client-sent id.
        private string CurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet("search-nutrition")]
        public async Task<IActionResult> SearchFoodNutrition([FromQuery] string search)
        {
            var list = await _foodNutritionRepository.SearchFoodNutrition(search);
            return Ok(list);
        }

        [HttpGet("get-nutrition-user")]
        public async Task<IActionResult> GetFoodNutritionUser()
        {
            var list = await _foodNutritionRepository.GetFoodNutritionUser(CurrentUserId());
            return Ok(list);
        }

        [HttpPost("cal-caloric-value")]
        public async Task<IActionResult> CalCaloricValue(AddFoodNutritionUser model)
        {
            var result = await _foodNutritionRepository.CalculateCaloric(model, CurrentUserId());
            if(result == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_NOT_FOUND" });
            }
            return Ok(result);
        }

        [HttpPut("update-foodnutrition-user")]
        public async Task<IActionResult> UpdateFoodNutritionUser(UpdateFoodNutritionUserDto model)
        {
            var result = await _foodNutritionRepository.UpdateFoodNutritionUser(model, CurrentUserId());
            if (result == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_USER_NOT_FOUND" });
            }
            return Ok(result);
        }

        [HttpDelete("delete-foodnutrition-user")]
        public async Task<IActionResult> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var deleted = await _foodNutritionRepository.DeleteFoodNutritionUser(model, CurrentUserId());
            if(deleted)
            {
                return Ok(new { message = "Successfully" });
            }
            return BadRequest(new { error = "DELETE_FAILED" });
        }
    }
}
