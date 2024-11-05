using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class HealthDatumRepository : GenericRepository<HealthDatum, string>
    {
        private readonly GymMarketContext _context;
        public HealthDatumRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
        // Override getAll method
        public override async Task<IEnumerable<HealthDatum>> GetAllAsync()
        {
            return await _context.HealthData
                .Include(h => h.HealthIndicators)
                .ToListAsync();
        }
        // Override getById method
        public override async Task<HealthDatum> GetByIdAsync(string id)
        {
            var healthDatum = await _context.HealthData
                .Include(h => h.HealthIndicators)
                .FirstOrDefaultAsync(h => h.HealthDataId == id);
            return healthDatum;
        }
        public async Task<IEnumerable<HealthDatum>> GetHealthDataByStudentIdAsync(string studentId)
        {
            return await _context.Set<HealthDatum>()
                .Where(h => h.StudentId == studentId)
                .Include(h => h.HealthIndicators)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();
        }
        // Lấy ra health Datum theo time
        public async Task<IEnumerable<IGrouping<DateTime, HealthDatum>>> GetHealthDataAggregatedByTimeAsync(string studentId, DateTime startDate, DateTime endDate)
        {
            var healthData = await _context.Set<HealthDatum>()
                .Where(h => h.StudentId == studentId && h.CreatedAt >= startDate && h.CreatedAt <= endDate)
                .Include(h => h.HealthIndicators)
                .GroupBy(h => h.CreatedAt.Value.Date) // Grouping by date
                .ToListAsync();
            return healthData;
        }
        // Lấy ra health Datum theo time
        public async Task<IEnumerable<object>> GetHealthDataSummaryByTimeAsync(string studentId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<HealthDatum>()
                .Where(h => h.StudentId == studentId && h.CreatedAt >= startDate && h.CreatedAt <= endDate)
                .GroupBy(h => h.CreatedAt.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AverageValue = g.SelectMany(h => h.HealthIndicators)
                                    .Average(ind => ParseIndicatorValue(ind.IndicatorValue))
                })
                .ToListAsync();
        }

        private double ParseIndicatorValue(string indicatorValue)
        {
            return double.TryParse(indicatorValue, out double val) ? val : 0;
        }
    }
}
