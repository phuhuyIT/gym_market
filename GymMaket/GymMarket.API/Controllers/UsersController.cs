using GymMarket.API.DTOs.User;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // Returns the caller's own profile (name, email, phone, address). The id
        // comes from the JWT so one user can't read another's PII — public trainer
        // contact details are served by the Trainer endpoint instead.
        [HttpGet("get-user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var res = await _userRepository.GetUserInfo(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            // Users may only update their own profile — never trust a client-sent id.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var res = await _userRepository.UpdateUser(userId, model);
            return StatusCode(res.StatusCode, res);
        }
    }
}
