using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ILectureMaterialRepository : IGenericRepository<LectureMaterial, string>
    {
        Task<IEnumerable<LectureMaterial>> GetMaterialsByLectureIdAsync(string lectureId);
    }
}
