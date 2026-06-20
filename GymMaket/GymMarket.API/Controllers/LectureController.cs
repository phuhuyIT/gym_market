using AutoMapper;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : GenericController<LectureCreateDTO, LectureUpdateDTO, Lecture, string>
    {
        private readonly ILectureRepository _lectureRepository;
        private readonly ICourseAccessService _courseAccessService;

        public LectureController(ILectureRepository repository, IMapper mapper, ICourseAccessService courseAccessService) : base(repository, mapper)
        {
            _lectureRepository = repository;
            _courseAccessService = courseAccessService;
        }

        protected override string GetEntityId(Lecture entity)
        {
            return entity.LectureId;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLecturesByCourseId(string courseId)
        {
            // Students may only study a course they have paid for; trainers, only their own.
            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var lectures = await _lectureRepository.GetLecturesByCourseIdAsync(courseId);
            var lectureDtos = _mapper.Map<IEnumerable<GetLectureDto>>(lectures);
            return Ok(lectureDtos);
        }

        // Bulk listing of every lecture is a staff-only operation.
        [HttpGet]
        [Authorize(Roles = "Trainer,Admin")]
        public override Task<IActionResult> GetAll() => base.GetAll();

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(string id)
        {
            if (!await _courseAccessService.CanAccessLectureAsync(User, id))
                return Forbid();

            return await base.GetById(id);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Create([FromBody] LectureCreateDTO createDto)
        {
            if (string.IsNullOrEmpty(createDto.CourseId))
                return BadRequest(new { error = "CourseId is required." });

            if (!await _courseAccessService.CanManageCourseAsync(User, createDto.CourseId))
                return Forbid();

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] LectureUpdateDTO updateDto)
        {
            if (!await _courseAccessService.CanManageLectureAsync(User, id))
                return Forbid();

            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Delete(string id)
        {
            if (!await _courseAccessService.CanManageLectureAsync(User, id))
                return Forbid();

            return await base.Delete(id);
        }
    }
}
