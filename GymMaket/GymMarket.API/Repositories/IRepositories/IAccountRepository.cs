using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IAccountRepository
    {
        // Existing
        Task<AppUser?> FindByEmail(string email);
        Task<IdentityResult> CreateUser(AppUser user, string password);
        Task<IdentityResult> AddToRole(AppUser user, string role);
        Task<AppUser?> FindByLoginAsync(string provider, string providerKey);
        Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo login);
        Task<IdentityResult> CreateUserWithoutPasswordAsync(AppUser user);
        Task CreateStudentAsync(Student student);
        Task CreateTrainerAsync(Trainer trainer);

        // User lookup
        Task<AppUser?> FindByIdAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(AppUser user);

        // Password
        Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<bool> HasPasswordAsync(AppUser user);

        // Email confirmation
        Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);
        Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);
        Task<bool> IsEmailConfirmedAsync(AppUser user);

        // 2FA
        Task<string?> GetAuthenticatorKeyAsync(AppUser user);
        Task ResetAuthenticatorKeyAsync(AppUser user);
        Task<bool> SetTwoFactorEnabledAsync(AppUser user, bool enabled);
        Task<bool> VerifyTwoFactorTokenAsync(AppUser user, string code);
        Task<bool> GetTwoFactorEnabledAsync(AppUser user);

        // Lockout
        Task<bool> IsLockedOutAsync(AppUser user);
        Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user);
        Task<int> GetAccessFailedCountAsync(AppUser user);
        Task ResetAccessFailedCountAsync(AppUser user);
        Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd);

        // Refresh tokens
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeAllUserRefreshTokensAsync(string userId);
    }
}
