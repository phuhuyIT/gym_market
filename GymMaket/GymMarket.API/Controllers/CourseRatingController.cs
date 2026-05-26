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
            var response = await _courseRatingRepository.AddRating(courseRatingCreateDTO);

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