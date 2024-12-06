using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRepository
    {
        Task<ICollection<Course>> GetCoursesOfTrainer(string trainerId);
    }
}
