using AutoMapper;
using GymMarket.API.DTOs.Course;
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

        }
    }
}
