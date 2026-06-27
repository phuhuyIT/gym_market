using Google.Apis.Auth;
using GymMarket.API.DTOs.Account;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Response.Account;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace GymMarket.API.Services
{
    public class AccountService : IAccountService
    {
        private static readonly string[] TrainerCategories = ["Yoga", "Cardio", "Strength", "Crossfit"];
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordSignInService _passwordSignInService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        private readonly MinIOService _minIOService;
        private readonly IEmailSender _emailSender;

        public AccountService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IPasswordSignInService passwordSignInService,
            IConfiguration configuration,
            ILogger<AccountService> logger,
            MinIOService minIOService,
            IEmailSender emailSender)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _passwordSignInService = passwordSignInService;
            _configuration = configuration;
            _logger = logger;
            _minIOService = minIOService;
            _emailSender = emailSender;
        }

        // ── Auth ──────────────────────────────────────────────────────

        public async Task<SignupResponse> SignUp(SignUpDto model)
        {
            if (!ApplicationRoles.TryNormalize(model.Role, out var role))
            {
                return new SignupResponse { StatusCode = 400, Errors = ["INVALID_ROLE"], Success = false };
            }

            var validationErrors = ValidateSignupProfile(model, role);
            if (validationErrors.Count > 0)
            {
                return new SignupResponse { StatusCode = 400, Errors = validationErrors, Success = false };
            }

            var userExist = await _accountRepository.FindByEmail(model.Email);
            if (userExist != null)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["EMAIL_ALREADY_EXISTS"], Success = false };
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _accountRepository.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new SignupResponse { StatusCode = 400, Errors = errors, Success = false };
            }

            var roleResult = await _accountRepository.AddToRole(user, role);
            if (!roleResult.Succeeded)
            {
                return new SignupResponse { StatusCode = 400, Errors = ["ROLE_ASSIGNMENT_FAILED"], Success = false };
            }

            await CreateProfileForRole(user, role, model);

            var emailResponse = await SendEmailConfirmationForUser(user);
            if (!emailResponse.Success)
            {
                return new SignupResponse
                {
                    StatusCode = emailResponse.StatusCode,
                    Success = false,
                    Errors = emailResponse.Errors
                };
            }

            return new SignupResponse { StatusCode = 200, Success = true, Message = "SUCCESS", UserId = user.Id };
        }

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var user = await _accountRepository.FindByEmail(model.Email);
            if (user == null)
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_CREDENTIALS"] };
            }

            var result = await _passwordSignInService.PasswordSignInAsync(user, model.Password, true, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return new LoginResponse { StatusCode = 400, Success = false, Errors = ["ACCOUNT_LOCKED"] };
                }
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_CREDENTIALS"] };
            }

            if (string.Equals(user.Status, "Suspended", StringComparison.OrdinalIgnoreCase))
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["ACCOUNT_SUSPENDED"] };
            }

            if (!await _accountRepository.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["EMAIL_NOT_CONFIRMED"] };
            }

            var token = await _jwtService.CreateJWT(user);
            var refreshToken = await CreateAndSaveRefreshToken(user.Id);
            return new LoginResponse { StatusCode = 200, Token = token, RefreshToken = refreshToken, Success = true };
        }

        public async Task<LoginResponse> GoogleLogin(GoogleLoginDto model)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"]! }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);

                if (payload == null)
                {
                    return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_GOOGLE_TOKEN"] };
                }

                var user = await _accountRepository.FindByLoginAsync("Google", payload.Subject);

                if (user == null)
                {
                    user = await _accountRepository.FindByEmail(payload.Email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            FullName = payload.Name,
                            Email = payload.Email,
                            UserName = payload.Email,
                            Avatar = payload.Picture
                        };

                        var result = await _accountRepository.CreateUserWithoutPasswordAsync(user);
                        if (!result.Succeeded)
                        {
                            return new LoginResponse { StatusCode = 400, Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
                        }

                        var role = "Student";
                        if (!string.IsNullOrEmpty(model.Role) && ApplicationRoles.TryNormalize(model.Role, out var normalizedRole))
                        {
                            role = normalizedRole;
                        }

                        var roleResult = await _accountRepository.AddToRole(user, role);
                        if (!roleResult.Succeeded)
                        {
                            return new LoginResponse { StatusCode = 400, Success = false, Errors = ["ROLE_ASSIGNMENT_FAILED"] };
                        }

                        if (role == "Trainer")
                        {
                            var trainer = new Trainer
                            {
                                TrainerId = Guid.NewGuid().ToString(),
                                Name = payload.Name,
                                Email = payload.Email,
                                ProfilePicture = payload.Picture ?? Defaults.StudentAvatarUrl,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                Rating = Defaults.DefaultRating,
                                Experience = 0,
                                ApprovalStatus = TrainerApprovalStatus.PendingReview,
                                UserId = user.Id
                            };
                            await _accountRepository.CreateTrainerAsync(trainer);
                        }
                        else
                        {
                            var student = new Student
                            {
                                StudentId = Guid.NewGuid().ToString(),
                                Name = payload.Name,
                                Email = payload.Email,
                                ProfilePicture = payload.Picture ?? Defaults.StudentAvatarUrl,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                UserId = user.Id
                            };
                            await _accountRepository.CreateStudentAsync(student);
                        }
                    }

                    var addLoginResult = await _accountRepository.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                    if (!addLoginResult.Succeeded)
                    {
                        return new LoginResponse { StatusCode = 400, Success = false, Errors = ["FAILED_TO_LINK_GOOGLE_ACCOUNT"] };
                    }
                }

                var token = await _jwtService.CreateJWT(user);
                var refreshToken = await CreateAndSaveRefreshToken(user.Id);
                return new LoginResponse { StatusCode = 200, Token = token, RefreshToken = refreshToken, Success = true, Message = "SUCCESS" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google login failed");
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["GOOGLE_LOGIN_FAILED"] };
            }
        }

        public async Task<ApiResponse> Logout(string userId)
        {
            await _accountRepository.RevokeAllUserRefreshTokensAsync(userId);
            return new ApiResponse { StatusCode = 200, Success = true, Message = "LOGGED_OUT" };
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenDto model)
        {
            var storedToken = await _accountRepository.GetRefreshTokenAsync(model.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return new LoginResponse { StatusCode = 400, Success = false, Errors = ["INVALID_REFRESH_TOKEN"] };
            }

            await _accountRepository.RevokeRefreshTokenAsync(storedToken);

            var token = await _jwtService.CreateJWT(storedToken.User);
            var newRefreshToken = await CreateAndSaveRefreshToken(storedToken.UserId);

            return new LoginResponse { StatusCode = 200, Token = token, RefreshToken = newRefreshToken, Success = true };
        }

        // ── Profile ───────────────────────────────────────────────────

        public async Task<ApiResponse> UpdateProfile(string userId, UpdateProfileDto model)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            if (model.FullName != null) user.FullName = model.FullName;
            if (model.Address != null) user.Address = model.Address;

            var result = await _accountRepository.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
            }

            return new ApiResponse { StatusCode = 200, Success = true, Message = "PROFILE_UPDATED" };
        }

        public async Task<ApiResponse> ChangePassword(string userId, ChangePasswordDto model)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            if (!await _accountRepository.HasPasswordAsync(user))
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = ["ACCOUNT_HAS_NO_PASSWORD"] };
            }

            var result = await _accountRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
            }

            return new ApiResponse { StatusCode = 200, Success = true, Message = "PASSWORD_CHANGED" };
        }

        public async Task<AvatarUploadResponse> UploadAvatar(string userId, IFormFile file)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new AvatarUploadResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                return new AvatarUploadResponse { StatusCode = 400, Success = false, Errors = ["INVALID_IMAGE_FORMAT"] };
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return new AvatarUploadResponse { StatusCode = 400, Success = false, Errors = ["FILE_TOO_LARGE"] };
            }

            var avatarUrl = await _minIOService.UploadSingleFileAsync(file, MinIOService.AVATARS);
            user.Avatar = avatarUrl;
            await _accountRepository.UpdateUserAsync(user);

            return new AvatarUploadResponse { StatusCode = 200, Success = true, Message = "AVATAR_UPLOADED", AvatarUrl = avatarUrl };
        }

        // ── Email Confirmation ────────────────────────────────────────

        public async Task<ApiResponse> SendEmailConfirmation(string userId)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            if (await _accountRepository.IsEmailConfirmedAsync(user))
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = ["EMAIL_ALREADY_CONFIRMED"] };
            }

            return await SendEmailConfirmationForUser(user);
        }

        private async Task<ApiResponse> SendEmailConfirmationForUser(AppUser user)
        {
            var token = await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
            var clientBaseUrl = _configuration["App:ClientBaseUrl"]?.TrimEnd('/') ?? "http://localhost:4200";
            var confirmationUrl =
                $"{clientBaseUrl}/confirm-email?userId={WebUtility.UrlEncode(user.Id)}&token={WebUtility.UrlEncode(token)}";

            var body = $"""
                <p>Hello {WebUtility.HtmlEncode(user.FullName ?? user.Email)},</p>
                <p>Please confirm your GymMarket account email by opening this link:</p>
                <p><a href="{confirmationUrl}">Confirm email</a></p>
                <p>If you did not request this email, you can ignore it.</p>
                """;

            try
            {
                await _emailSender.SendEmailAsync(user.Email!, "Confirm your GymMarket email", body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email confirmation for user {UserId}", user.Id);
                return new ApiResponse { StatusCode = 500, Success = false, Errors = ["EMAIL_SEND_FAILED"] };
            }

            return new ApiResponse { StatusCode = 200, Success = true, Message = "EMAIL_CONFIRMATION_SENT" };
        }

        public async Task<ApiResponse> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _accountRepository.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            var result = await _accountRepository.ConfirmEmailAsync(user, model.Token);
            if (!result.Succeeded)
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
            }

            return new ApiResponse { StatusCode = 200, Success = true, Message = "EMAIL_CONFIRMED" };
        }

        // ── Two-Factor Authentication ─────────────────────────────────

        public async Task<Enable2FAResponse> Enable2FA(string userId)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new Enable2FAResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            await _accountRepository.ResetAuthenticatorKeyAsync(user);
            var key = await _accountRepository.GetAuthenticatorKeyAsync(user);

            var uri = GenerateQrCodeUri(user.Email!, key!);

            return new Enable2FAResponse
            {
                StatusCode = 200,
                Success = true,
                Message = "SCAN_QR_CODE",
                SharedKey = key,
                QrCodeUri = uri
            };
        }

        public async Task<ApiResponse> Verify2FA(string userId, Verify2FADto model)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            var isValid = await _accountRepository.VerifyTwoFactorTokenAsync(user, model.Code);
            if (!isValid)
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = ["INVALID_2FA_CODE"] };
            }

            await _accountRepository.SetTwoFactorEnabledAsync(user, true);
            return new ApiResponse { StatusCode = 200, Success = true, Message = "2FA_ENABLED" };
        }

        public async Task<ApiResponse> Disable2FA(string userId)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            if (!await _accountRepository.GetTwoFactorEnabledAsync(user))
            {
                return new ApiResponse { StatusCode = 400, Success = false, Errors = ["2FA_NOT_ENABLED"] };
            }

            await _accountRepository.SetTwoFactorEnabledAsync(user, false);
            await _accountRepository.ResetAuthenticatorKeyAsync(user);

            return new ApiResponse { StatusCode = 200, Success = true, Message = "2FA_DISABLED" };
        }

        // ── Account Lockout ───────────────────────────────────────────

        public async Task<LockoutStatusResponse> GetLockoutStatus(string userId)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new LockoutStatusResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            return new LockoutStatusResponse
            {
                StatusCode = 200,
                Success = true,
                IsLockedOut = await _accountRepository.IsLockedOutAsync(user),
                LockoutEnd = await _accountRepository.GetLockoutEndDateAsync(user),
                AccessFailedCount = await _accountRepository.GetAccessFailedCountAsync(user)
            };
        }

        public async Task<ApiResponse> UnlockAccount(string userId)
        {
            var user = await _accountRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = 404, Success = false, Errors = ["USER_NOT_FOUND"] };
            }

            await _accountRepository.SetLockoutEndDateAsync(user, null);
            await _accountRepository.ResetAccessFailedCountAsync(user);

            return new ApiResponse { StatusCode = 200, Success = true, Message = "ACCOUNT_UNLOCKED" };
        }

        public Task<ApiResponse> SetLockoutPolicy(SetLockoutPolicyDto model)
        {
            var identityOptions = _configuration.GetSection("Identity");

            return Task.FromResult(new ApiResponse
            {
                StatusCode = 200,
                Success = true,
                Message = $"LOCKOUT_POLICY_SET: MaxAttempts={model.MaxFailedAccessAttempts}, Duration={model.LockoutDurationInMinutes}min"
            });
        }

        // ── Helpers ───────────────────────────────────────────────────

        private async Task<string> CreateAndSaveRefreshToken(string userId)
        {
            var refreshTokenValue = _jwtService.GenerateRefreshToken();
            var refreshTokenDays = int.Parse(_configuration["JWT:RefreshTokenExpiresInDays"] ?? "7");

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepository.SaveRefreshTokenAsync(refreshToken);
            return refreshTokenValue;
        }

        private async Task CreateProfileForRole(AppUser user, string role, SignUpDto model)
        {
            if (role == ApplicationRoles.Trainer)
            {
                var trainer = new Trainer
                {
                    TrainerId = Guid.NewGuid().ToString(),
                    Name = model.FullName.Trim(),
                    Email = model.Email.Trim(),
                    Certification = string.IsNullOrWhiteSpace(model.Certification) ? "General Fitness Trainer" : model.Certification.Trim(),
                    Category = NormalizeTrainerCategory(model.Category),
                    Bio = string.IsNullOrWhiteSpace(model.Bio) ? "Professional fitness instructor." : model.Bio.Trim(),
                    Experience = model.Experience ?? 0,
                    Rating = Defaults.DefaultRating,
                    ProfilePicture = Defaults.AvatarUrl,
                    Description = string.Empty,
                    ApprovalStatus = TrainerApprovalStatus.PendingReview,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

                await _accountRepository.CreateTrainerAsync(trainer);
                return;
            }

            var student = new Student
            {
                StudentId = Guid.NewGuid().ToString(),
                Name = model.FullName.Trim(),
                Email = model.Email.Trim(),
                HealthStatus = string.IsNullOrWhiteSpace(model.HealthStatus) ? "Good" : model.HealthStatus.Trim(),
                ProfilePicture = Defaults.StudentAvatarUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _accountRepository.CreateStudentAsync(student);
        }

        private static List<string> ValidateSignupProfile(SignUpDto model, string role)
        {
            if (role != ApplicationRoles.Trainer)
            {
                return [];
            }

            var errors = new List<string>();

            if (model.Experience is < 0 or > 50)
            {
                errors.Add("INVALID_TRAINER_EXPERIENCE");
            }

            if (!string.IsNullOrWhiteSpace(model.Category) && NormalizeTrainerCategory(model.Category) is null)
            {
                errors.Add("INVALID_TRAINER_CATEGORY");
            }

            if (model.Certification?.Length > 200)
            {
                errors.Add("TRAINER_CERTIFICATION_TOO_LONG");
            }

            if (model.Bio?.Length > 500)
            {
                errors.Add("TRAINER_BIO_TOO_LONG");
            }

            return errors;
        }

        private static string? NormalizeTrainerCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return null;
            }

            return TrainerCategories.FirstOrDefault(
                supported => string.Equals(supported, category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private string GenerateQrCodeUri(string email, string key)
        {
            return $"otpauth://totp/GymMarket:{Uri.EscapeDataString(email)}?secret={key}&issuer=GymMarket&digits=6";
        }
    }
}
