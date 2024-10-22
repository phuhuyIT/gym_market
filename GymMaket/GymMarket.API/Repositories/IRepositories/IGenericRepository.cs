namespace GymMarket.API.Repositories.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Get(string id);
        Task<IReadOnlyList<T>> GetAll();
        Task<bool> Exists(string id);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
