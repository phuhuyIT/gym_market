using GymMarket.API.DTOs.User;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("get-user-info/{userId}")]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            var res = await _userRepository.GetUserInfo(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            var res = await _userRepository.UpdateUser(model);
            return StatusCode(res.StatusCode, res);
        }
    }
}
