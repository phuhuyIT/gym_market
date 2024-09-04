using GymMarket.API.DTOs.Account;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            var response = await accountRepository.SignUp(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }        
    }
}
