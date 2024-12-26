using AutoMapper;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRatingController: ControllerBase
    {
        private readonly CourseRatingRepository courseRatingRepository;

        public CourseRatingController(CourseRatingRepository courseRatingRepository)
        {
            this.courseRatingRepository = courseRatingRepository;
        }


        [HttpPost("add-course-rating")]
        public async Task<IActionResult> AddCourseRating(CourseRatingCreateDto courseRatingCreateDTO)
        {
            var resposen = await courseRatingRepository.AddCourseRating(courseRatingCreateDTO);
            return StatusCode(resposen.StatusCode, resposen.Message);
        }

        [HttpGet("get-course-rating/{courseId}")]
        public async Task<IActionResult> GetCourseRatings(string courseId)
        {
            var ratings = await courseRatingRepository.GetCourseRatings(courseId);
            return Ok(ratings);
        }
    }
}