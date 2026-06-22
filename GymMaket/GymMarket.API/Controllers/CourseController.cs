using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : GenericController<CourseCreateDTO, CourseUpdateDTO, Course, string>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAccessService _courseAccessService;

        public CourseController(IGenericRepository<Course, string> repository,
            IMapper mapper, ICourseRepository courseRepository,
            ICourseAccessService courseAccessService
            ) : base(repository, mapper)
        {
            _courseRepository = courseRepository;
            _courseAccessService = courseAccessService;
        }

        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }

        // Creating and modifying courses are staff operations; a trainer may only
        // touch their own courses (admins may touch any — CanManageCourseAsync).

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Create([FromBody] CourseCreateDTO createDto)
        {
            // A trainer always creates courses they own — never trust a client-sent id.
            // Admins may create a course on another trainer's behalf.
            var trainerId = User.FindFirstValue("trainerId");
            if (!string.IsNullOrEmpty(trainerId))
            {
                createDto.TrainerId = trainerId;
            }
            createDto.Status = CourseStatus.Normalize(createDto.Status);

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] CourseUpdateDTO updateDto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, id))
                return Forbid();

            if (updateDto.Status != null)
                updateDto.Status = CourseStatus.Normalize(updateDto.Status);
            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Delete(string id)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, id))
                return Forbid();

            return await base.Delete(id);
        }

        [AllowAnonymous]
        [HttpGet("search-and-filter")]
        public async Task<IActionResult> SearchAndFilterCourses(
            [FromQuery] string? keyword,
            [FromQuery] string? coachName,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? minDuration,
            [FromQuery] int? maxDuration,
            [FromQuery] double? minRating,
            [FromQuery] string? category)
        {
            var courses = await _courseRepository.SearchAndFilterCoursesAsync(keyword, coachName, minPrice, maxPrice, minDuration, maxDuration, minRating, category);
            return Ok(courses);
        }

        [HttpPut("update-course")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> UpdateCourse([FromForm] CourseUpdateDTO courseUpdate)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseUpdate.CourseId))
                return Forbid();

            if (courseUpdate.Status != null)
                courseUpdate.Status = CourseStatus.Normalize(courseUpdate.Status);
            var res = await _courseRepository.UpdateCourse(courseUpdate);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }

        [AllowAnonymous]
        [HttpGet("get-courses-of-trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesOfTrainer(string trainerId)
        {
            var courses = await _courseRepository.GetCoursesOfTrainer(trainerId);
            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("get-course/{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var course = await _courseRepository.GetCourse(id);

            if (course == null)
            {
                return BadRequest(new { errors = new string[] { "COURSE_NOT_FOUND" } });
            }
            var isPublished = string.IsNullOrWhiteSpace(course.Status) || course.Status == CourseStatus.Published;
            if (!isPublished && !await _courseAccessService.CanManageCourseAsync(User, id))
            {
                return BadRequest(new { errors = new string[] { "COURSE_NOT_FOUND" } });
            }
            return Ok(course);
        }

        [AllowAnonymous]
        [HttpGet("get-courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string? category = null, [FromQuery] string? searchString = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = Defaults.PageSize)
        {
            var courses = await _courseRepository.GetCourses(pageIndex, pageSize, searchString, category);

            if (courses.Items.Count == 0 && courses.TotalCount == 0)
            {
                return Ok(courses);
            }
            return Ok(courses);
        }

    }
}
