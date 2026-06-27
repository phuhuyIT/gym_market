using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : GenericController<CourseCreateDTO, CourseUpdateDTO, Course, string>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAccessService _courseAccessService;
        private readonly GymMarketContext _context;

        public CourseController(IGenericRepository<Course, string> repository,
            IMapper mapper, ICourseRepository courseRepository,
            ICourseAccessService courseAccessService,
            GymMarketContext context
            ) : base(repository, mapper)
        {
            _courseRepository = courseRepository;
            _courseAccessService = courseAccessService;
            _context = context;
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
            var normalizedStatus = NormalizeCreateStatus(createDto.Status);
            if (normalizedStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_COURSE_STATUS" } });

            if (!User.IsInRole(ApplicationRoles.Admin) &&
                !await IsTrainerApprovedAsync(createDto.TrainerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "TRAINER_NOT_APPROVED" } });
            }

            if (!User.IsInRole(ApplicationRoles.Admin) &&
                !CourseStatus.IsTrainerWritable(normalizedStatus))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "COURSE_REVIEW_REQUIRED" } });
            }

            createDto.Status = normalizedStatus;

            if (createDto.Status == CourseStatus.Published &&
                !await IsTrainerApprovedAsync(createDto.TrainerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "TRAINER_NOT_APPROVED" } });
            }

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] CourseUpdateDTO updateDto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, id))
                return Forbid();

            if (updateDto.Status != null)
            {
                var normalizedStatus = CourseStatus.TryNormalize(updateDto.Status);
                if (normalizedStatus == null)
                    return BadRequest(new { success = false, errors = new[] { "INVALID_COURSE_STATUS" } });

                if (!User.IsInRole(ApplicationRoles.Admin) &&
                    !CourseStatus.IsTrainerWritable(normalizedStatus))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "COURSE_REVIEW_REQUIRED" } });
                }

                updateDto.Status = normalizedStatus;
            }

            if (updateDto.Status == CourseStatus.Published &&
                !await IsCourseTrainerApprovedAsync(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "TRAINER_NOT_APPROVED" } });
            }

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
            {
                var normalizedStatus = CourseStatus.TryNormalize(courseUpdate.Status);
                if (normalizedStatus == null)
                    return BadRequest(new { success = false, errors = new[] { "INVALID_COURSE_STATUS" } });

                if (!User.IsInRole(ApplicationRoles.Admin) &&
                    !CourseStatus.IsTrainerWritable(normalizedStatus))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "COURSE_REVIEW_REQUIRED" } });
                }

                courseUpdate.Status = normalizedStatus;
            }

            if (courseUpdate.Status == CourseStatus.Published &&
                !await IsCourseTrainerApprovedAsync(courseUpdate.CourseId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "TRAINER_NOT_APPROVED" } });
            }

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

        [HttpGet("admin-review")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCoursesForReview(
            [FromQuery] string? status,
            [FromQuery] string? search,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = Defaults.PageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 100) pageSize = 100;

            var normalizedStatus = CourseStatus.TryNormalize(status);
            if (!string.IsNullOrWhiteSpace(status) && normalizedStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_COURSE_STATUS_FILTER" } });

            search = search?.Trim();

            var query = _context.Courses
                .AsNoTracking()
                .Include(c => c.Trainer)
                .AsQueryable();

            if (normalizedStatus != null)
            {
                query = normalizedStatus == CourseStatus.Published
                    ? query.Where(c => c.Status == CourseStatus.Published || c.Status == null || c.Status == "")
                    : query.Where(c => c.Status == normalizedStatus);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    (c.Title != null && c.Title.Contains(search)) ||
                    (c.Category != null && c.Category.Contains(search)) ||
                    (c.Trainer != null && c.Trainer.Name != null && c.Trainer.Name.Contains(search)) ||
                    (c.Trainer != null && c.Trainer.Email != null && c.Trainer.Email.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Status == CourseStatus.PendingReview ? 0 : 1)
                .ThenBy(c => c.Title)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new AdminCourseListItemDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    Type = c.Type,
                    Category = c.Category,
                    Price = c.Price,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = string.IsNullOrWhiteSpace(c.Status) ? CourseStatus.Published : c.Status!,
                    TrainerId = c.TrainerId,
                    TrainerName = c.Trainer != null ? c.Trainer.Name : null,
                    TrainerEmail = c.Trainer != null ? c.Trainer.Email : null,
                    TrainerApprovalStatus = c.Trainer == null
                        ? null
                        : TrainerApprovalStatus.NormalizeStored(c.Trainer.ApprovalStatus)
                })
                .ToListAsync();

            return Ok(new PagedResult<AdminCourseListItemDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }

        [HttpPut("{id}/moderation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ModerateCourse(string id, UpdateCourseModerationDto model)
        {
            var normalizedStatus = CourseStatus.TryNormalize(model.Status);
            if (normalizedStatus == null || !CourseStatus.ModerationStatuses.Contains(normalizedStatus))
                return BadRequest(new { success = false, errors = new[] { "INVALID_COURSE_STATUS" } });

            var course = await _context.Courses
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
                return NotFound(new { success = false, errors = new[] { "COURSE_NOT_FOUND" } });

            if (normalizedStatus == CourseStatus.Published &&
                !TrainerApprovalStatus.IsApproved(course.Trainer?.ApprovalStatus))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, errors = new[] { "TRAINER_NOT_APPROVED" } });
            }

            course.Status = normalizedStatus;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "COURSE_MODERATION_UPDATED" });
        }

        private string? NormalizeCreateStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return User.IsInRole(ApplicationRoles.Admin)
                    ? CourseStatus.Published
                    : CourseStatus.PendingReview;

            return CourseStatus.TryNormalize(status);
        }

        private async Task<bool> IsCourseTrainerApprovedAsync(string courseId)
        {
            var trainerId = await _context.Courses
                .AsNoTracking()
                .Where(c => c.CourseId == courseId)
                .Select(c => c.TrainerId)
                .FirstOrDefaultAsync();

            return await IsTrainerApprovedAsync(trainerId);
        }

        private async Task<bool> IsTrainerApprovedAsync(string? trainerId)
        {
            if (string.IsNullOrWhiteSpace(trainerId))
                return false;

            var status = await _context.Trainers
                .AsNoTracking()
                .Where(t => t.TrainerId == trainerId)
                .Select(t => t.ApprovalStatus)
                .FirstOrDefaultAsync();

            return TrainerApprovalStatus.IsApproved(status);
        }
    }
}
