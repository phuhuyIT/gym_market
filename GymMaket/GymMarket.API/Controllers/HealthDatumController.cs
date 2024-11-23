using AutoMapper;
using GymMarket.API.DTOs.HealthDatum;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthDatumController : GenericController<HealthDatumCreateDTO, HealthDatumUpdateDTO, HealthDatum, string>
    {
        private readonly IHealthDatumRepository _healthDatumRepository;
        public HealthDatumController(IGenericRepository<HealthDatum, string> repository, IMapper mapper) : base(repository, mapper)
        {
            _healthDatumRepository = repository as IHealthDatumRepository ?? throw new ArgumentNullException(nameof(repository), "Repository must be of type IHealthDatumRepository.");
        }

        protected override string GetEntityId(HealthDatum entity)
        {
            return entity.HealthDataId;
        }
        public async Task<IActionResult> GetHealthDatumOfTrainer(string trainerId)
        {
            // check if trainerId is null or empty
            if (string.IsNullOrWhiteSpace(trainerId))
                return BadRequest("Trainer ID cannot be null or empty.");

            // Add your logic here to fetch health data of the trainer
            var healthData = await _healthDatumRepository.GetHealthDataByStudentIdAsync(trainerId);
            if (healthData == null)
                return NotFound("Health data not found for the given trainer ID.");

            return Ok(healthData);
        }
        public async Task<IActionResult> GetHealthDatumOfStudentByTime(HealthDatumByTimeQuery info)
        {
            // check if studentId is null or empty
            if (string.IsNullOrWhiteSpace(info.StudentId))
                return BadRequest("Student ID cannot be null or empty.");

            // Add your logic here to fetch health data of the student
            var healthData = await _healthDatumRepository.GetHealthDataAggregatedByTimeAsync(info.StudentId, info.StartDate, info.EndDate);
            if (healthData == null)
                return BadRequest("Health data not found for the given student ID.");

            return Ok(healthData);
        }
        public async Task<IActionResult> GetHealthDatumSummaryByTime(HealthDatumByTimeQuery info)
        {
            // check if studentId is null or empty
            if (string.IsNullOrWhiteSpace(info.StudentId))
                return BadRequest("Student ID cannot be null or empty.");

            // Add your logic here to fetch health data summary of the student
            var healthData = await _healthDatumRepository.GetHealthDataSummaryByTimeAsync(info.StudentId, info.StartDate, info.EndDate);
            if (healthData == null)
                return BadRequest("Health data not found for the given student ID.");

            return Ok(healthData);
        }
    }
}
