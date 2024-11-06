using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IAccountRepository:IGenericRepository<AccountRepository>
    {
        Task<ApiResponse> SignUp(SignUpDto model);
        Task<LoginResponse> Login(LoginDto model);
    }
}
