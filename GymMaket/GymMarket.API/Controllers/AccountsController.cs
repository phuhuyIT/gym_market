using GymMarket.API.DTOs.Account;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            var response = await _accountService.SignUp(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.UserId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var response = await _accountService.Login(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.Token });
        }
    }
}
