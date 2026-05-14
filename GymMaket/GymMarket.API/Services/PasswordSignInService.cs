using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Services
{
    public class PasswordSignInService : IPasswordSignInService
    {
        private readonly SignInManager<AppUser> _signInManager;

        public PasswordSignInService(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task<SignInResult> PasswordSignInAsync(
            AppUser user,
            string password,
            bool isPersistent,
            bool lockoutOnFailure)
        {
            return _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}
