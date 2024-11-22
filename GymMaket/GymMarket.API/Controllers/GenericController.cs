using AutoMapper;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GenericController<TCreateDto, TUpdateDto, TEntity, TKey> : ControllerBase where TEntity : class where TUpdateDto: class where TCreateDto : class
    {
        protected readonly IGenericRepository<TEntity, TKey> _repository;
        protected readonly IMapper _mapper;

        protected GenericController(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/[controller]
        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        // GET: api/[controller]/{id}
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        // POST: api/[controller]
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<TCreateDto, TEntity>(createDto);
            await _repository.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = GetEntityId(entity) }, entity);
        }

        // PUT: api/[controller]/{id}
        [HttpPut("{id}")]
        public virtual IActionResult Update([FromBody] TUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<TUpdateDto, TEntity>(updateDto);
            _repository.Update(entity);
            return NoContent();
        }

        // DELETE: api/[controller]/{id}
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            _repository.Remove(entity);
            return NoContent();
        }

        // Helper method to get entity ID - should be implemented by derived classes
        protected abstract TKey GetEntityId(TEntity entity);
    }

}
