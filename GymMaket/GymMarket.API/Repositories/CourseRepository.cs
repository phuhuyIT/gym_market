﻿using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRepository : GenericRepository<Course, string>, ICourseRepository
    {
        private readonly GymMarketContext _context;
        public CourseRepository(GymMarketContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context;
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
    }
}
