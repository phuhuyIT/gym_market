using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel.Notification;

namespace GymMarket.API.Repositories
{
    public class CourseRepository : GenericRepository<Course, string>, ICourseRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;
        private readonly MinIOService minIOService;

        public CourseRepository(GymMarketContext context, IMapper mapper, MinIOService minIOService) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            this.minIOService = minIOService;
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


        async Task<ICollection<Course>> ICourseRepository.GetCoursesOfTrainer(string trainerId)
        {
            var courses = await _context.Courses.AsNoTrackingWithIdentityResolution()
                .Where(c => c.TrainerId == trainerId)
                .ToListAsync();

            return courses;
        }

        public async Task<ApiResponse> UpdateCourse(CourseUpdateDTO courseUpdateDTO)
        {
            // map TUpdateDto to TEntity
            var mapEntity = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);



            if (courseUpdateDTO.Images.Count > 0)
            {
                var oldFiles = await _context.FileCourses.AsNoTrackingWithIdentityResolution()
                          .Where(c => c.CourseId == courseUpdateDTO.CourseId && c.TypeFile == "IMAGE")
                          .ToListAsync();
                _context.FileCourses.RemoveRange(oldFiles);
                await _context.SaveChangesAsync();
            }

            if (courseUpdateDTO.Videos.Count > 0)
            {
                var oldFiles = await _context.FileCourses.AsNoTrackingWithIdentityResolution()
                          .Where(c => c.CourseId == courseUpdateDTO.CourseId && c.TypeFile == "VIDEO")
                          .ToListAsync();
                _context.FileCourses.RemoveRange(oldFiles);
                await _context.SaveChangesAsync();
            }

            var rAddFile = await minIOService.UploadFiles(new DTOs.FileMinIO.FileAdd
            {
                CourseId = courseUpdateDTO.CourseId,
                Images = courseUpdateDTO.Images,
                Videos = courseUpdateDTO.Videos,
            });

            _context.Courses.Update(mapEntity);

            var r = await _context.SaveChangesAsync();
            if (r > 0)
            {
                return new ApiResponse
                {
                    Errors = [],
                    Message = "Cập nhật thành công",
                    StatusCode = 200,
                    Success = true
                };
            }
            return new ApiResponse
            {
                Errors = [],
                Message = "Cập nhật thất bại. Vui lòng thử lại.",
                StatusCode = 400,
                Success = false
            };
        }

        public async Task<GetCourseDto?> GetCourse(string courseId)
        {
            var course = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.CourseId == courseId)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return null;
            }

            var courseDto = _mapper.Map<Course, GetCourseDto>(course);
            var courseFiles = await _context.FileCourses
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.CourseId == courseId)
                .ToListAsync();

            var courseFileDtos = _mapper.Map<List<FileCourse>, List<GetFileDto>>(courseFiles);
            courseDto.GetFileDtos = courseFileDtos;
            return courseDto;
        }

        public async Task<List<GetCourseDto>> GetCourses()
        {
            var course = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();


            var courseDto = _mapper.Map<List<Course>, List<GetCourseDto>>(course);

            foreach(var c in courseDto)
            {
                var courseFiles = await _context.FileCourses
                   .AsNoTrackingWithIdentityResolution()
                   .Where(course => course.CourseId == c.CourseId)
                   .ToListAsync();

                var courseFileImages = courseFiles.Where(c => c.TypeFile == "IMAGE").ToList();

                var courseFileDtos = _mapper.Map<List<FileCourse>, List<GetFileDto>>(courseFileImages);
                c.GetFileDtos = courseFileDtos;
            }
           
            return courseDto;
        }

        public async Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? decription, decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category)
        {
            var query = _context.Courses.AsQueryable();

            // Tìm kiếm theo tên khóa học hoặc chủ đề
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Type.Contains(keyword) || c.Category.Contains(keyword));
            }

            // Tìm kiếm theo tên coach
            if (!string.IsNullOrEmpty(decription))
            {
                query = query.Where(c => c.Description.Contains(decription));
            }

            // Lọc theo mức giá
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // Lọc theo thời lượng khóa học
            if (minDuration.HasValue)
            {
                query = query.Where(c => c.Duration >= minDuration.Value);
            }
            if (maxDuration.HasValue)
            {
                query = query.Where(c => c.Duration <= maxDuration.Value);
            }



            // Lọc theo thể loại khóa học
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Equals(category));
            }

            return await query.ToListAsync();

        }
    }
}
