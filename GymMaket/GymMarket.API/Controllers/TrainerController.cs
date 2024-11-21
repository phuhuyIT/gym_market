using AutoMapper;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : GenericController<TrainerCreateDTO, TrainerUpdateDTO, Trainer, string>
    {
        public TrainerController(IGenericRepository<Trainer, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string GetEntityId(Trainer entity)
        {
            return entity.TrainerId;
        }
    }
}
