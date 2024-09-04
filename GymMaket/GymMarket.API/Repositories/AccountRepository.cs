using GymMarket.API.Data;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly GymMarketContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public AccountRepository(GymMarketContext context, 
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<ApiResponse> SignUp(SignUpDto model)
        {
            var userExist = await userManager.FindByEmailAsync(model.Email);
            if (userExist != null)
            {
                return new ApiResponse { StatusCode = 400, Errors = ["Email này đã tồn tại. Vui lòng sử dụng email khác"], Success = false };
            }

            var role = await roleManager.FindByNameAsync(model.Role);
            if (role == null)
            {
                return new ApiResponse { StatusCode = 400, Errors = ["Vai trò không tồn tại"], Success = false };
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var rAddUser = await userManager.CreateAsync(user, model.Password);
            if(rAddUser.Succeeded == false)
            {
                var errors = rAddUser.Errors.Select(e => e.Description).ToList();
                return new ApiResponse { StatusCode = 400, Errors = errors, Success = false };
            }



            return new ApiResponse { StatusCode = 200, Success = true, Message = "Đăng ký thành công" };
        }
    }
}
