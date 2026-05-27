using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly GymMarketContext _context;

        public AccountRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, GymMarketContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        public async Task CreateStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTrainerAsync(Trainer trainer)
        {
            await _context.Trainers.AddAsync(trainer);
            await _context.SaveChangesAsync();
        }

        // User lookup

        public async Task<AppUser?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        // Password

        public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> HasPasswordAsync(AppUser user)
        {
            return await _userManager.HasPasswordAsync(user);
        }

        // Email confirmation

        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> IsEmailConfirmedAsync(AppUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        // 2FA

        public async Task<string?> GetAuthenticatorKeyAsync(AppUser user)
        {
            return await _userManager.GetAuthenticatorKeyAsync(user);
        }

        public async Task ResetAuthenticatorKeyAsync(AppUser user)
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
        }

        public async Task<bool> SetTwoFactorEnabledAsync(AppUser user, bool enabled)
        {
            var result = await _userManager.SetTwoFactorEnabledAsync(user, enabled);
            return result.Succeeded;
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(AppUser user, string code)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(AppUser user)
        {
            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        // Lockout

        public async Task<bool> IsLockedOutAsync(AppUser user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user)
        {
            return await _userManager.GetLockoutEndDateAsync(user);
        }

        public async Task<int> GetAccessFailedCountAsync(AppUser user)
        {
            return await _userManager.GetAccessFailedCountAsync(user);
        }

        public async Task ResetAccessFailedCountAsync(AppUser user)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
        }

        public async Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd)
        {
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        }

        // Refresh tokens

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllUserRefreshTokensAsync(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
