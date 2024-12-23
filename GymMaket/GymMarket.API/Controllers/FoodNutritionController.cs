using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodNutritionController : ControllerBase
    {
        private readonly FoodNutritionRepository foodNutritionRepository;

        public FoodNutritionController(FoodNutritionRepository foodNutritionRepository)
        {
            this.foodNutritionRepository = foodNutritionRepository;
        }

        [HttpGet("search-nutrition")]
        public async Task<IActionResult> SearchFoodNutrition([FromQuery] string search)
        {
            var list = await foodNutritionRepository.SearchFoodNutrition(search);
            return Ok(list);
        }

        [HttpGet("get-nutrition-user/{userId}")]
        public async Task<IActionResult> GetFoodNutritionUser(string userId)
        {
            var list = await foodNutritionRepository.GetFoodNutritionUser(userId);
            return Ok(list);
        }

        [HttpPost("cal-caloric-value")]
        public async Task<IActionResult> CalCaloricValue(AddFoodNutritionUser model)
        {
            var r = await foodNutritionRepository.CalculateCaloric(model);
            if(r == null)
            {
                return BadRequest(r);
            }
            return Ok(r);
        }
    }
}
