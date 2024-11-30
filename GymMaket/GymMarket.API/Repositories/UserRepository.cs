using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Response.User;
using GymMarket.API.DTOs.User;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<GetUserInfoResponse> GetUserInfo(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new GetUserInfoResponse { Errors = ["Không tìm thấy người dùng"], StatusCode = 404, Success = false };
            }

            var userInfo = new GetUserInfoDto
            {
                Address = user.Address,
                Avatar = user.Avatar,
                Email = user.Email,
                FullName = user.FullName,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status
            };
            return new GetUserInfoResponse { Errors = [], StatusCode = 200, Success = true, UserInfo = userInfo };
        }

        public async Task<ApiResponse> UpdateUser(UpdateUserDto model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return new ApiResponse { Errors = ["Không tìm thấy người dùng"], StatusCode = 404, Success = false };
            }

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.Avatar = model.Avatar;
            user.Status = model.Status;
            user.PhoneNumber = model.PhoneNumber;

            var r = await userManager.UpdateAsync(user);
            if (r.Succeeded == true)
            {
                return new ApiResponse { Errors = [], StatusCode = 200, Success = true };
            }
            return new ApiResponse { Errors = ["Cập nhật thất bại. Vui lòng thử lại."], StatusCode = 400, Success = false };
        }
    }
}
