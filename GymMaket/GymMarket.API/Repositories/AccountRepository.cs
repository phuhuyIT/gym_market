using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly GymMarketContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly JWTService jWTService;
        private readonly UserManager<AppUser> userManager;

        public AccountRepository(GymMarketContext context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            JWTService jWTService,
            UserManager<AppUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.jWTService = jWTService;
            this.userManager = userManager;
        }

        public async Task<SignupResponse> SignUp(SignUpDto model)
        {
            var userExist = await userManager.FindByEmailAsync(model.Email);
            if (userExist != null)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["Email này đã tồn tại. Vui lòng sử dụng email khác"], Success = false };
            }

            var role = await roleManager.FindByNameAsync(model.Role);
            if (role == null)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["Vai trò không tồn tại"], Success = false };
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var rAddUser = await userManager.CreateAsync(user, model.Password);
            if (rAddUser.Succeeded == false)
            {
                var errors = rAddUser.Errors.Select(e => e.Description).ToList();
                return new SignupResponse { StatusCode = 400, Errors = errors, Success = false };
            }
            await userManager.AddToRoleAsync(user, model.Role);
            return new SignupResponse { StatusCode = 200, Success = true, Message = "Đăng ký thành công", UserId = user.Id };
        }

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["Email hoặc mật khẩu không chính xác"] };
            }

            var rSignIn = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (rSignIn.Succeeded == false)
            {
                if (rSignIn.IsLockedOut)
                {
                    return new LoginResponse { StatusCode = 400, Success = false, Errors = [$"Tài khoản bị khóa đến {user.LockoutEnd!.Value.ToString("dd/MM/yyyyy HH:mm")}"] };
                }
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["Email hoặc mật khẩu không chính xác"] };
            }

            var token = await jWTService.CreateJWT(user);
            return new LoginResponse { StatusCode = 200, Token = token, Success = true };
        }
    }
}
