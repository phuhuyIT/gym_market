using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.DTOs.CourseModule;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.FoodNutrition;
using GymMarket.API.DTOs.FoodNutritionUser;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.DTOs.LectureMaterial;
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
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Course, CourseUpdateDTO>();

            CreateMap<TrainerCreateDTO, Trainer>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TrainerUpdateDTO, Trainer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Trainer, TrainerUpdateDTO>();

            CreateMap<CourseOptionCreateDTO, CourseOption>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CourseOptionUpdateDTO, CourseOption>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CourseOption, CourseOptionUpdateDTO>();

            CreateMap<StudentCreateDTO, Student>()
               .ReverseMap()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StudentUpdateDTO, Student>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Student, StudentUpdateDTO>();

            CreateMap<CourseRatingCreateDto, CourseRating>()
              .ReverseMap();
            CreateMap<CourseRegistrationCreateDto, CourseRegistration>()
            .ReverseMap();
            CreateMap<Course, GetCourseDto>()
                .ForMember(dest => dest.GetFileDtos, opt => opt.MapFrom(src => src.FileCourses))
                .ReverseMap();

            CreateMap<CourseModuleCreateDto, CourseModule>();
            CreateMap<CourseModuleUpdateDto, CourseModule>();
            CreateMap<CourseModule, CourseModuleDto>();

            CreateMap<FileCourse, GetFileDto>()
                .ReverseMap();

            // Lecture mappings
            CreateMap<LectureCreateDTO, Lecture>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => LearningActivityType.Normalize(src.ActivityType)));
            CreateMap<LectureUpdateDTO, Lecture>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => LearningActivityType.Normalize(src.ActivityType)));
            CreateMap<Lecture, GetLectureDto>()
                .ForMember(dest => dest.ModuleTitle, opt => opt.MapFrom(src => src.Module != null ? src.Module.Title : null))
                .ForMember(dest => dest.ModuleOrder, opt => opt.MapFrom(src => src.Module != null ? src.Module.Order : null));

            // LectureMaterial mappings
            CreateMap<LectureMaterialCreateDTO, LectureMaterial>().ReverseMap();
            CreateMap<LectureMaterialUpdateDTO, LectureMaterial>().ReverseMap();
            CreateMap<LectureMaterial, GetLectureMaterialDto>().ReverseMap();

            // FoodNutrition mappings (macros on the log entry are computed by
            // FoodNutritionService, not mapped from the request)
            CreateMap<CreateFoodNutritionDto, FoodNutrition>();
            CreateMap<UpdateFoodNutritionDto, FoodNutrition>();
            CreateMap<AddFoodNutritionUser, FoodNutritionUser>();
            CreateMap<NutritionBudgetDto, NutritionBudget>().ReverseMap();
        }
    }
}
