using AutoMapper;
using GymMarket.API.DTOs.Student;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : GenericController<StudentCreateDTO, StudentUpdateDTO, Student, string>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(repository, mapper)
        {
            _userManager = userManager;
            _studentRepository = repository;
        }

        // A profile contains PII (email, phone, address, health status), so these
        // endpoints only ever serve the authenticated user's own record — the user
        // id comes from the JWT, never from the client.
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _studentRepository.GetStudentProfileByUserId(userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(string id, [FromBody] StudentUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(updateDto.Password))
            {
                ModelState.AddModelError("Password", "Password is required to confirm changes.");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(existingEntity.UserId))
            {
                ModelState.AddModelError("User", "Associated user account not found.");
                return BadRequest(ModelState);
            }

            var appUser = await _userManager.FindByIdAsync(existingEntity.UserId);
            if (appUser == null)
            {
                ModelState.AddModelError("User", "Associated user account not found.");
                return BadRequest(ModelState);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(appUser, updateDto.Password);
            if (!isPasswordValid)
            {
                ModelState.AddModelError("Password", "Incorrect password. Please try again.");
                return BadRequest(ModelState);
            }

            // Set password to null so AutoMapper does not map it onto the Student entity columns
            updateDto.Password = null;

            _mapper.Map(updateDto, existingEntity);
            await _repository.Update(existingEntity);
            return NoContent();
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var students = await _repository.FindAsync(s => s.UserId == userId);
            var student = students.FirstOrDefault();
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [Authorize(Roles = "Trainer,Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = Defaults.PageSize)
        {
            var isAdmin = User.IsInRole(ApplicationRoles.Admin);
            var trainerId = User.FindFirstValue("trainerId");

            var result = await _studentRepository.SearchStudents(
                pageIndex,
                pageSize,
                search,
                trainerId,
                includeAllStudents: isAdmin);

            return Ok(result);
        }

        protected override string GetEntityId(Student entity)
        {
            return entity.StudentId;
        }
    }
}
