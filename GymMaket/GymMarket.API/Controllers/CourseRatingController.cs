using System.Security.Claims;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseRatingController: ControllerBase
    {
        private readonly ICourseRatingRepository _courseRatingRepository;

        public CourseRatingController(ICourseRatingRepository courseRatingRepository)
        {
            _courseRatingRepository = courseRatingRepository;
        }


        [HttpPost("add-course-rating")]
        public async Task<IActionResult> AddCourseRating(CourseRatingCreateDto courseRatingCreateDTO)
        {
            // The reviewer is always the authenticated student; never trust an id from the body.
            var studentId = User.FindFirstValue("studentId");
            if (string.IsNullOrEmpty(studentId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "NOT_ENROLLED");
            }

            var response = await _courseRatingRepository.AddRating(courseRatingCreateDTO, studentId);

            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpGet("get-course-rating/{courseId}")]
        public async Task<IActionResult> GetCourseRatings(string courseId)
        {
            var ratings = await _courseRatingRepository.GetRatingsByCourseId(courseId);
            return Ok(ratings);
        }
    }
}