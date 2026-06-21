using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class TrainerRepository : GenericRepository<Trainer, string>, ITrainerRepository
    {
        private readonly GymMarketContext _context;

        public TrainerRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<PagedResult<TrainerSearchDto>> SearchTrainers(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? category = null,
            bool? eliteOnly = null)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 50) pageSize = 50;

            search = search?.Trim();
            category = category?.Trim();

            var query = _context.Trainers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    (t.Name != null && t.Name.Contains(search)) ||
                    (t.Email != null && t.Email.Contains(search)) ||
                    (t.Certification != null && t.Certification.Contains(search)) ||
                    (t.Bio != null && t.Bio.Contains(search)) ||
                    t.Description.Contains(search) ||
                    (t.AppUser != null && t.AppUser.FullName != null && t.AppUser.FullName.Contains(search)) ||
                    (t.AppUser != null && t.AppUser.Email != null && t.AppUser.Email.Contains(search)) ||
                    (t.AppUser != null && t.AppUser.PhoneNumber != null && t.AppUser.PhoneNumber.Contains(search)) ||
                    (t.AppUser != null && t.AppUser.Status != null && t.AppUser.Status.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t =>
                    (t.Certification != null && t.Certification.Contains(category)) ||
                    (t.Bio != null && t.Bio.Contains(category)) ||
                    t.Description.Contains(category));
            }

            if (eliteOnly == true)
            {
                query = query.Where(t => t.Experience >= 8);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.AppUser != null ? t.AppUser.FullName : t.Name)
                .ThenBy(t => t.TrainerId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TrainerSearchDto
                {
                    TrainerId = t.TrainerId,
                    UserId = t.UserId,
                    Name = t.Name,
                    Email = t.Email,
                    FullName = t.AppUser != null ? t.AppUser.FullName : null,
                    Certification = t.Certification,
                    Bio = t.Bio,
                    Experience = t.Experience,
                    Rating = t.Rating,
                    ProfilePicture = t.ProfilePicture,
                    Status = t.AppUser != null ? t.AppUser.Status : null,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<TrainerSearchDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
