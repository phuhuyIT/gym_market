using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ITrainerRepository : IGenericRepository<Trainer, string>
    {
        Task<PagedResult<TrainerSearchDto>> SearchTrainers(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? category = null,
            bool? eliteOnly = null,
            decimal? minRating = null,
            decimal? maxRating = null,
            int? minExperience = null,
            int? maxExperience = null,
            string? status = null);
    }
}
