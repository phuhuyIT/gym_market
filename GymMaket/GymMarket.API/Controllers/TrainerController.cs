using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                trainer.CreatedAt = DateTime.UtcNow;
                trainer.UpdatedAt = DateTime.UtcNow;
                var createTrainer = await _trainerRepository.Add(trainer);

                return Ok(new { message = "Trainer created successfully.", trainerId = createTrainer.TrainerId });
            }
            catch (DbUpdateException dbEx)
            {
               
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return BadRequest($"Database update error: {innerMessage}");
            }
            catch (Exception ex)
            {
                // Bắt các lỗi khác
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            var trainers = await _trainerRepository.Get(Id);
            if (trainers == null)
            {
                return BadRequest("Trainer data is null");
            }
            return Ok(trainers);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(string Id,[FromBody] Trainer trainer)
        {
            if (trainer == null)
            {
                return BadRequest();
            }
            var existingTrainer = await _trainerRepository.Get(Id);
            if (existingTrainer == null)
            {
                return NotFound($"Trainer with Id {Id} not found");
            }
            try
            {

                await _trainerRepository.Update(existingTrainer);
                return Ok("Trainer is Update Successfull");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string Id)
        {
            var existingTrainer = await _trainerRepository.Get(Id);
            if (existingTrainer == null)
            {
                return NotFound($"Trainer with Id {Id} not found");
            }
            await _trainerRepository.Delete(existingTrainer);
            return Ok("Đã Xóa Thành Công");
        }


    }
}
