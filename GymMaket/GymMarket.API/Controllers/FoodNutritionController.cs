using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodNutritionController : ControllerBase
    {
        private readonly FoodNutritionRepository _foodNutritionRepository;

        public FoodNutritionController(FoodNutritionRepository foodNutritionRepository)
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
            var r = await _foodNutritionRepository.CalculateCaloric(model);
            if(r == null)
            {
                return BadRequest(r);
            }
            return Ok(r);
        }

        [HttpPost("delete-foodnutrition-user")]
        public async Task<IActionResult> DeleteFoodNutritionUser(DeleteFoodNutritionUserDto model)
        {
            var r = await _foodNutritionRepository.DeleteFoodNutritionUser(model);
            if(r == true)
            {
                return Ok(new { message = "Successfully" });
            }
            return BadRequest(r);
        }
    }
}
