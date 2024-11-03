using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync<TCreateDto>(TCreateDto entity) where TCreateDto:class;
        Task AddRangeAsync<TCreateDto>(IEnumerable<TCreateDto> entities) where TCreateDto:class;
        void Update<TUpdateDto>(TUpdateDto entity)where TUpdateDto:class;
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
