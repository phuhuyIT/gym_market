using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response.Account;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IAccountRepository
    {
        Task<SignupResponse> SignUp(SignUpDto model);
        Task<LoginResponse> Login(LoginDto model);
    }
}
