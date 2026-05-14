using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Services
{
    public interface IPasswordSignInService
    {
        Task<SignInResult> PasswordSignInAsync(
            AppUser user,
            string password,
            bool isPersistent,
            bool lockoutOnFailure);
    }
}
