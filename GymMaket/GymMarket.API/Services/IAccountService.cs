using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Response.Account;

namespace GymMarket.API.Services
{
    public interface IAccountService
    {
        // Auth
        Task<SignupResponse> SignUp(SignUpDto model);
        Task<LoginResponse> Login(LoginDto model);
        Task<LoginResponse> GoogleLogin(GoogleLoginDto model);
        Task<ApiResponse> Logout(string userId);
        Task<LoginResponse> RefreshToken(RefreshTokenDto model);

        // Profile
        Task<ApiResponse> UpdateProfile(string userId, UpdateProfileDto model);
        Task<ApiResponse> ChangePassword(string userId, ChangePasswordDto model);
        Task<AvatarUploadResponse> UploadAvatar(string userId, IFormFile file);

        // Email Confirmation
        Task<ApiResponse> SendEmailConfirmation(string userId);
        Task<ApiResponse> ConfirmEmail(ConfirmEmailDto model);

        // Two-Factor Authentication
        Task<Enable2FAResponse> Enable2FA(string userId);
        Task<ApiResponse> Verify2FA(string userId, Verify2FADto model);
        Task<ApiResponse> Disable2FA(string userId);

        // Account Lockout
        Task<LockoutStatusResponse> GetLockoutStatus(string userId);
        Task<ApiResponse> UnlockAccount(string userId);
        Task<ApiResponse> SetLockoutPolicy(SetLockoutPolicyDto model);
    }
}
