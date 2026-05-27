using GymMarket.API.DTOs.Account;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // ── Auth (anonymous) ──────────────────────────────────────────

        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            var response = await _accountService.SignUp(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.UserId });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var response = await _accountService.Login(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.Token, response.RefreshToken });
        }

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto model)
        {
            var response = await _accountService.GoogleLogin(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.Token, response.RefreshToken });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto model)
        {
            var response = await _accountService.RefreshToken(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.Token, response.RefreshToken });
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var response = await _accountService.ConfirmEmail(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        // ── Authenticated endpoints ───────────────────────────────────

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _accountService.Logout(GetUserId());
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto model)
        {
            var response = await _accountService.UpdateProfile(GetUserId(), model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var response = await _accountService.ChangePassword(GetUserId(), model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "NO_FILE_PROVIDED", success = false });
            }
            var response = await _accountService.UploadAvatar(GetUserId(), file);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.AvatarUrl });
        }

        [Authorize]
        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation()
        {
            var response = await _accountService.SendEmailConfirmation(GetUserId());
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        // ── 2FA ───────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2FA()
        {
            var response = await _accountService.Enable2FA(GetUserId());
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.SharedKey, response.QrCodeUri });
        }

        [Authorize]
        [HttpPost("2fa/verify")]
        public async Task<IActionResult> Verify2FA(Verify2FADto model)
        {
            var response = await _accountService.Verify2FA(GetUserId(), model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable2FA()
        {
            var response = await _accountService.Disable2FA(GetUserId());
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        // ── Lockout (admin) ───────────────────────────────────────────

        [Authorize(Roles = "Admin")]
        [HttpGet("lockout/{userId}")]
        public async Task<IActionResult> GetLockoutStatus(string userId)
        {
            var response = await _accountService.GetLockoutStatus(userId);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success, response.IsLockedOut, response.LockoutEnd, response.AccessFailedCount });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("lockout/{userId}/unlock")]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var response = await _accountService.UnlockAccount(userId);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("lockout/policy")]
        public async Task<IActionResult> SetLockoutPolicy(SetLockoutPolicyDto model)
        {
            var response = await _accountService.SetLockoutPolicy(model);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        // ── Helpers ───────────────────────────────────────────────────

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
    }
}
