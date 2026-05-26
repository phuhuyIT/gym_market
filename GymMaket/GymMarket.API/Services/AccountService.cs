using Google.Apis.Auth;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response.Account;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordSignInService _passwordSignInService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IPasswordSignInService passwordSignInService,
            IConfiguration configuration,
            ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _passwordSignInService = passwordSignInService;
            _configuration = configuration;
            _logger = logger;
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

        public async Task<LoginResponse> GoogleLogin(GoogleLoginDto model)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"]! }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);

                if (payload == null)
                {
                    return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_GOOGLE_TOKEN"] };
                }

                var user = await _accountRepository.FindByLoginAsync("Google", payload.Subject);

                if (user == null)
                {
                    user = await _accountRepository.FindByEmail(payload.Email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            FullName = payload.Name,
                            Email = payload.Email,
                            UserName = payload.Email,
                            Avatar = payload.Picture
                        };

                        var result = await _accountRepository.CreateUserWithoutPasswordAsync(user);
                        if (!result.Succeeded)
                        {
                            return new LoginResponse { StatusCode = 400, Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
                        }

                        // Determine the role (default to Student)
                        var role = "Student";
                        if (!string.IsNullOrEmpty(model.Role) && ApplicationRoles.TryNormalize(model.Role, out var normalizedRole))
                        {
                            role = normalizedRole;
                        }

                        var roleResult = await _accountRepository.AddToRole(user, role);
                        if (!roleResult.Succeeded)
                        {
                            return new LoginResponse { StatusCode = 400, Success = false, Errors = ["ROLE_ASSIGNMENT_FAILED"] };
                        }

                        // Create corresponding role entity
                        if (role == "Trainer")
                        {
                            var trainer = new Trainer
                            {
                                TrainerId = Guid.NewGuid().ToString(),
                                Name = payload.Name,
                                Email = payload.Email,
                                ProfilePicture = payload.Picture ?? Defaults.StudentAvatarUrl,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                Rating = Defaults.DefaultRating,
                                Experience = 0,
                                UserId = user.Id
                            };
                            await _accountRepository.CreateTrainerAsync(trainer);
                        }
                        else
                        {
                            var student = new Student
                            {
                                StudentId = Guid.NewGuid().ToString(),
                                Name = payload.Name,
                                Email = payload.Email,
                                ProfilePicture = payload.Picture ?? Defaults.StudentAvatarUrl,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                UserId = user.Id
                            };
                            await _accountRepository.CreateStudentAsync(student);
                        }
                    }

                    var addLoginResult = await _accountRepository.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                    if (!addLoginResult.Succeeded)
                    {
                        return new LoginResponse { StatusCode = 400, Success = false, Errors = ["FAILED_TO_LINK_GOOGLE_ACCOUNT"] };
                    }
                }

                var token = await _jwtService.CreateJWT(user);
                return new LoginResponse { StatusCode = 200, Token = token, Success = true, Message = "SUCCESS" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google login failed");
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["GOOGLE_LOGIN_FAILED"] };
            }
        }
    }
}
