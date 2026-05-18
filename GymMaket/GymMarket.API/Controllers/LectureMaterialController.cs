using AutoMapper;
using GymMarket.API.DTOs.LectureMaterial;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureMaterialController : GenericController<LectureMaterialCreateDTO, LectureMaterialUpdateDTO, LectureMaterial, string>
    {
        private readonly ILectureMaterialRepository _lectureMaterialRepository;

        public LectureMaterialController(ILectureMaterialRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _lectureMaterialRepository = repository;
        }

        protected override string GetEntityId(LectureMaterial entity)
        {
            return entity.MaterialId;
        }

        [HttpGet("lecture/{lectureId}")]
        public async Task<IActionResult> GetMaterialsByLectureId(string lectureId)
        {
            var materials = await _lectureMaterialRepository.GetMaterialsByLectureIdAsync(lectureId);
            var materialDtos = _mapper.Map<IEnumerable<GetLectureMaterialDto>>(materials);
            return Ok(materialDtos);
        }
    }
}
