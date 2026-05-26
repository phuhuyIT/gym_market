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

        public override async Task<Course> GetByIdAsync(string id)
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
            var mapEntity = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (courseUpdateDTO.Images.Count > 0)
                {
                    var oldImageFiles = await _context.FileCourses
                              .Where(c => c.CourseId == courseUpdateDTO.CourseId && c.TypeFile == "IMAGE")
                              .ToListAsync();
                    _context.FileCourses.RemoveRange(oldImageFiles);
                }

                if (courseUpdateDTO.Videos.Count > 0)
                {
                    var oldVideoFiles = await _context.FileCourses
                              .Where(c => c.CourseId == courseUpdateDTO.CourseId && c.TypeFile == "VIDEO")
                              .ToListAsync();
                    _context.FileCourses.RemoveRange(oldVideoFiles);
                }

                await _minioService.UploadFiles(new DTOs.FileMinIO.FileAdd
                {
                    CourseId = courseUpdateDTO.CourseId,
                    Images = courseUpdateDTO.Images,
                    Videos = courseUpdateDTO.Videos,
                });

                _context.Courses.Update(mapEntity);

                var r = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
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

        public async Task<List<GetCourseDto>> GetCourses(int pageIndex = 1, int pageSize = 15, string? searchString = null, string? category = null)
        {
            var courses = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.FileCourses)
                .Where(c =>
                            ((string.IsNullOrEmpty(searchString) != true && c.Title!.ToLower().Contains(searchString.ToLower())) || string.IsNullOrEmpty(searchString) == true)
                         && ((string.IsNullOrEmpty(category) != true && c.Category!.ToLower().Contains(category.ToLower())) || string.IsNullOrEmpty(category) == true)
                )
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<Course>, List<GetCourseDto>>(courses);
        }

        public async Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? description, decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category)
        {
            var query = _context.Courses.AsQueryable();

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
