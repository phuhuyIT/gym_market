using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response.Account;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;

namespace GymMarket.API.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordSignInService _passwordSignInService;

        public AccountService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IPasswordSignInService passwordSignInService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _passwordSignInService = passwordSignInService;
        }

        public async Task<SignupResponse> SignUp(SignUpDto model)
        {
            if (!ApplicationRoles.TryNormalize(model.Role, out var role))
            {
                return new SignupResponse { StatusCode = 400, Errors = ["INVALID_ROLE"], Success = false };
            }

            var userExist = await _accountRepository.FindByEmail(model.Email);
            if (userExist != null)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["EMAIL_ALREADY_EXISTS"], Success = false };
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _accountRepository.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new SignupResponse { StatusCode = 400, Errors = errors, Success = false };
            }

            var roleResult = await _accountRepository.AddToRole(user, role);
            if (!roleResult.Succeeded)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["ROLE_ASSIGNMENT_FAILED"], Success = false };
            }

            return new SignupResponse { StatusCode = 200, Success = true, Message = "SUCCESS", UserId = user.Id };
        }

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var user = await _accountRepository.FindByEmail(model.Email);
            if (user == null)
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_CREDENTIALS"] };
            }

            var result = await _passwordSignInService.PasswordSignInAsync(user, model.Password, true, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return new LoginResponse { StatusCode = 400, Success = false, Errors = ["ACCOUNT_LOCKED"] };
                }
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_CREDENTIALS"] };
            }

            var token = await _jwtService.CreateJWT(user);
            return new LoginResponse { StatusCode = 200, Token = token, Success = true };
        }
    }
}
