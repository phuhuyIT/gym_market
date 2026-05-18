using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AppUser?> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUser(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRole(AppUser user, string role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role {role} does not exist." });
            }
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<AppUser?> FindByLoginAsync(string provider, string providerKey)
        {
            return await _userManager.FindByLoginAsync(provider, providerKey);
        }

        public async Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            return await _userManager.AddLoginAsync(user, login);
        }

        public async Task<IdentityResult> CreateUserWithoutPasswordAsync(AppUser user)
        {
            return await _userManager.CreateAsync(user);
        }
    }
}
