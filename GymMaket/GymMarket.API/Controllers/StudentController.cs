using AutoMapper;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : GenericController<Student, string>
    {
        public StudentController(IGenericRepository<Student, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string GetEntityId(Student entity)
        {
            return entity.StudentId;
        }
    }
}
