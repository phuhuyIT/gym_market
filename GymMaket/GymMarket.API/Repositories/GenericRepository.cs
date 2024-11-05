using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        private readonly GymMarketContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IMapper _mapper;

        public GenericRepository(GymMarketContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync<TCreateDto>(TCreateDto entity) where TCreateDto : class
        {
            // map TCreateDto to TEntity
            var mapEntity = _mapper.Map<TCreateDto, TEntity>(entity);
            await _dbSet.AddAsync(mapEntity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddRangeAsync<TCreateDto>(IEnumerable<TCreateDto> entities) where TCreateDto : class
        {
            // map TCreateDto to TEntity
            var mapEntities = _mapper.Map<IEnumerable<TCreateDto>, IEnumerable<TEntity>>(entities);
            await _dbSet.AddRangeAsync(mapEntities);
            await _context.SaveChangesAsync();
        }

        public void Update<TUpdateDto>(TUpdateDto entity) where TUpdateDto : class
        {
            // map TUpdateDto to TEntity
            var mapEntity = _mapper.Map<TUpdateDto, TEntity>(entity);
            _dbSet.Attach(mapEntity);
            _context.Entry(mapEntity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }
    }
}
