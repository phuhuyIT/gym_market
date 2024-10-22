using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerRepository _trainerRepository;
        
        public TrainerController(ITrainerRepository trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }
        [HttpGet("top-trainers")]
        public async Task<IActionResult> GetTopTrainers()
        {
            var trainers = await _trainerRepository.GetTrainersAsync();
            var topTrainers = trainers.OrderByDescending(t => t.Rating).Take(5).ToList();
            return Ok(topTrainers);
        }
        [HttpGet("get-lists")]
        public async Task<IActionResult> GetList()
        {
            var trainers = await _trainerRepository.GetAll();
            return Ok(trainers);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Trainer trainer)
        {
            if (trainer == null)
            {
                return BadRequest("Trainer data is null");
            }
            try
            {
                var createTrainer = await _trainerRepository.Add(trainer);
                return Ok("Created");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //[HttpPut("update")]
        //public async Task<IActionResult> Update([FromForm] Trainer trainer)
        //{
        //    if (trainer == null)
        //    {
        //        return
        //    }
        //}


    }
}
