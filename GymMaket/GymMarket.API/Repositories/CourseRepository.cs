using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRepository : GenericRepository<Course, string>, ICourseRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;
        private readonly MinIOService _minioService;

        public CourseRepository(GymMarketContext context, IMapper mapper, MinIOService minioService) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _minioService = minioService;
        }

        //override getall method
        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.CourseRatings)
                .Include(c => c.Lectures)
                .Include(c => c.Trainer)
                .ToListAsync();
        }
        //override getbyid method

        public override async Task<Course?> GetByIdAsync(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseRatings)
                .Include(c => c.Lectures)
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            return course;
        }

        async Task<ICollection<GetCourseDto>> ICourseRepository.GetCoursesOfTrainer(string trainerId)
        {
            var courses = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.FileCourses)
                .Where(c => c.TrainerId == trainerId)
                .ToListAsync();

            return _mapper.Map<List<Course>, List<GetCourseDto>>(courses);
        }

        public async Task<ApiResponse> UpdateCourse(CourseUpdateDTO courseUpdateDTO)
        {
            // The DTO carries no TrainerId; keep the current owner so the full-entity
            // Update below can't clear (or transfer) course ownership.
            var currentCourse = await _context.Courses
                .AsNoTracking()
                .Where(c => c.CourseId == courseUpdateDTO.CourseId)
                .Select(c => new { c.TrainerId, c.Status })
                .FirstOrDefaultAsync();

            if (currentCourse == null)
            {
                return new ApiResponse
                {
                    Errors = ["COURSE_NOT_FOUND"],
                    StatusCode = 404,
                    Success = false
                };
            }

            var mapEntity = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);
            mapEntity.TrainerId = currentCourse.TrainerId;
            mapEntity.Status = CourseStatus.Normalize(courseUpdateDTO.Status ?? currentCourse.Status);

            var useTransaction = _context.Database.IsRelational();
            await using var transaction = useTransaction
                ? await _context.Database.BeginTransactionAsync()
                : null;
            try
            {
                var retainedImageIds = courseUpdateDTO.RetainedImageObjectIds.ToHashSet(StringComparer.Ordinal);
                var oldImageFiles = await _context.FileCourses
                          .Where(c => c.CourseId == courseUpdateDTO.CourseId
                              && c.TypeFile == FileType.Image
                              && !retainedImageIds.Contains(c.ObjectId))
                          .ToListAsync();
                _context.FileCourses.RemoveRange(oldImageFiles);

                var retainedVideoIds = courseUpdateDTO.RetainedVideoObjectIds.ToHashSet(StringComparer.Ordinal);
                var oldVideoFiles = await _context.FileCourses
                          .Where(c => c.CourseId == courseUpdateDTO.CourseId
                              && c.TypeFile == FileType.Video
                              && !retainedVideoIds.Contains(c.ObjectId))
                          .ToListAsync();
                _context.FileCourses.RemoveRange(oldVideoFiles);

                await _minioService.UploadFiles(new DTOs.FileMinIO.FileAdd
                {
                    CourseId = courseUpdateDTO.CourseId,
                    Images = courseUpdateDTO.Images,
                    Videos = courseUpdateDTO.Videos,
                });

                _context.Courses.Update(mapEntity);

                // Keep open (unpaid) payments in step with the new price, so a student who
                // registered earlier but hasn't paid yet owes the current amount. Paid
                // payments are never touched — a settled amount must not change when the
                // price changes. This mirrors the lazy re-sync in
                // CourseRegistrationRepository.GetCoursePaymentInfo.
                var openPayments = await _context.Payments
                    .Where(p => p.CourseId == courseUpdateDTO.CourseId
                             && (p.PaymentStatus == PaymentStatus.Pending
                              || p.PaymentStatus == PaymentStatus.NotStarted))
                    .ToListAsync();
                foreach (var payment in openPayments)
                {
                    var optionsAmount = await _context.CourseRegistrationOptions
                        .Where(ro => ro.Registration != null
                            && ro.Registration.StudentId == payment.StudentId
                            && ro.Registration.CourseId == courseUpdateDTO.CourseId)
                        .SumAsync(ro => ro.Option != null ? ro.Option.Price ?? 0 : 0);
                    var newPrice = (mapEntity.Price ?? 0) + (mapEntity.AdditionalPrice ?? 0) + optionsAmount;
                    if (payment.PaymentAmount != newPrice)
                    {
                        payment.PaymentAmount = newPrice;
                        payment.UpdatedAt = DateTime.UtcNow;
                    }
                }

                var r = await _context.SaveChangesAsync();
                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                if (r > 0)
                {
                    return new ApiResponse
                    {
                        Errors = [],
                        Message = "SUCCESS",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ApiResponse
                {
                    Errors = ["COURSE_UPDATE_FAILED"],
                    StatusCode = 400,
                    Success = false
                };
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
        }

        public async Task<GetCourseDto?> GetCourse(string courseId)
        {
            var course = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.FileCourses)
                .Where(c => c.CourseId == courseId)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return null;
            }

            return _mapper.Map<Course, GetCourseDto>(course);
        }

        public async Task<PagedResult<GetCourseDto>> GetCourses(int pageIndex = 1, int pageSize = Defaults.PageSize, string? searchString = null, string? category = null)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 50) pageSize = 50;

            searchString = searchString?.Trim();
            category = category?.Trim();

            var query = _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.FileCourses)
                .Where(c => c.Status == null || c.Status == "" || c.Status == CourseStatus.Published)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c =>
                    (c.Title != null && c.Title.Contains(searchString)) ||
                    (c.Description != null && c.Description.Contains(searchString)) ||
                    (c.Type != null && c.Type.Contains(searchString)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(c => c.Category != null && c.Category.Contains(category));
            }

            var totalCount = await query.CountAsync();
            var courses = await query
                .OrderBy(c => c.Title)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<GetCourseDto>
            {
                Items = _mapper.Map<List<Course>, List<GetCourseDto>>(courses),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
            };
        }

        public async Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? description, decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category)
        {
            var query = _context.Courses
                .Where(c => c.Status == null || c.Status == "" || c.Status == CourseStatus.Published)
                .AsQueryable();

            // Search by course title or topic
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Type!.Contains(keyword) || c.Category!.Contains(keyword));
            }

            // Search by trainer description
            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(c => c.Description!.Contains(description));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // Filter by course duration
            if (minDuration.HasValue)
            {
                query = query.Where(c => c.Duration >= minDuration.Value);
            }
            if (maxDuration.HasValue)
            {
                query = query.Where(c => c.Duration <= maxDuration.Value);
            }



            // Filter by course category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category!.Equals(category));
            }

            return await query.ToListAsync();

        }
    }
}
