using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IHealthDatumRepository : IGenericRepository<HealthDatum, string>
    {
        Task<IEnumerable<HealthDatum>> GetHealthDataByStudentIdAsync(string studentId);
        Task<IEnumerable<IGrouping<DateTime, HealthDatum>>> GetHealthDataAggregatedByTimeAsync(string studentId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<object>> GetHealthDataSummaryByTimeAsync(string studentId, DateTime startDate, DateTime endDate);
    }
}
