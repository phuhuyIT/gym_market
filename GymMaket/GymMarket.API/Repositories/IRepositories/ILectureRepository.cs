using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ILectureRepository : IGenericRepository<Lecture, string>
    {
        Task<IEnumerable<Lecture>> GetLecturesByCourseIdAsync(string courseId);
    }
}
