using GymMarket.API.Models;

namespace GymMarket.API.Services
{
    public interface IJwtService
    {
        Task<string> CreateJWT(AppUser user);
        string GenerateRefreshToken();
    }
}
