using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Response.User;
using GymMarket.API.DTOs.User;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<GetUserInfoResponse> GetUserInfo(string userId);
        Task<ApiResponse> UpdateUser(UpdateUserDto model);
    }
}
