using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ITrainerRepository:IGenericRepository<Trainer>
    {
        Task<List<Trainer>> GetTrainersAsync();
         Task<List<Trainer>> GetTopRatedTrainersAsync(int topCount);
    }
}
