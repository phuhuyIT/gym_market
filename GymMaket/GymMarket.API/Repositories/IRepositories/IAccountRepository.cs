using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IAccountRepository
    {
        Task<AppUser?> FindByEmail(string email);
        Task<IdentityResult> CreateUser(AppUser user, string password);
        Task<IdentityResult> AddToRole(AppUser user, string role);
        Task<AppUser?> FindByLoginAsync(string provider, string providerKey);
        Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo login);
        Task<IdentityResult> CreateUserWithoutPasswordAsync(AppUser user);
        Task CreateStudentAsync(Student student);
        Task CreateTrainerAsync(Trainer trainer);
    }
}
