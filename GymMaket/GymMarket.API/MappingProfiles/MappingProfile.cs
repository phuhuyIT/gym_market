using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.Student;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;

namespace GymMarket.API.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CourseCreateDTO, Course>()
                .ReverseMap()
                .ForMember(dest => dest.CourseId,
                    opt => opt.MapFrom(src => Guid.NewGuid().ToString()));

            CreateMap<CourseUpdateDTO, Course>()
                .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TrainerCreateDTO, Trainer>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TrainerUpdateDTO, Trainer>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CourseOptionCreateDTO, CourseOption>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CourseOptionUpdateDTO, CourseOption>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StudentCreateDTO, Student>()
               .ReverseMap()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StudentUpdateDTO, Student>()
              .ReverseMap()
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CourseRatingCreateDTO, CourseRating>()
              .ReverseMap();

            CreateMap<Course, GetCourseDto>()
                .ReverseMap();

            CreateMap<FileCourse, GetFileDto>()
                .ReverseMap();
        }
    }
}
