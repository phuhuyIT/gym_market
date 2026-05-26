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

        [HttpGet("search-nutrition")]
        public async Task<IActionResult> SearchFoodNutrition([FromQuery] string search)
        {
            var list = await _foodNutritionRepository.SearchFoodNutrition(search);
            return Ok(list);
        }

        [HttpGet("get-nutrition-user/{userId}")]
        public async Task<IActionResult> GetFoodNutritionUser(string userId)
        {
            var list = await _foodNutritionRepository.GetFoodNutritionUser(userId);
            return Ok(list);
        }

        [HttpPost("cal-caloric-value")]
        public async Task<IActionResult> CalCaloricValue(AddFoodNutritionUser model)
        {
            var result = await _foodNutritionRepository.CalculateCaloric(model);
            if(result == null)
            {
                return BadRequest(new { error = "FOOD_NUTRITION_NOT_FOUND" });
            }
            return Ok(result);
        }

        [HttpDelete("delete-foodnutrition-user")]
        public async Task<IActionResult> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var deleted = await _foodNutritionRepository.DeleteFoodNutritionUser(model);
            if(deleted)
            {
                return Ok(new { message = "Successfully" });
            }
            return BadRequest(new { error = "DELETE_FAILED" });
        }
    }
}
