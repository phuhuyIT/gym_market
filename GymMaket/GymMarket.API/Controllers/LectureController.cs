using AutoMapper;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : GenericController<LectureCreateDTO, LectureUpdateDTO, Lecture, string>
    {
        private readonly ILectureRepository _lectureRepository;

        public LectureController(ILectureRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _lectureRepository = repository;
        }

        protected override string GetEntityId(Lecture entity)
        {
            return entity.LectureId;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLecturesByCourseId(string courseId)
        {
            var lectures = await _lectureRepository.GetLecturesByCourseIdAsync(courseId);
            var lectureDtos = _mapper.Map<IEnumerable<GetLectureDto>>(lectures);
            return Ok(lectureDtos);
        }
    }
}
