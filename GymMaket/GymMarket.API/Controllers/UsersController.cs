using GymMarket.API.DTOs.User;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet("get-user-info/{userId}")]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            var res = await userRepository.GetUserInfo(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            var res = await userRepository.UpdateUser(model);
            return StatusCode(res.StatusCode, res);
        }
    }
}
