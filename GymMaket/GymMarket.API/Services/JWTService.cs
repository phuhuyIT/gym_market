using GymMarket.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymMarket.API.Services
{
    public class JWTService
    {
        private readonly IConfiguration configuration;
        private readonly SymmetricSecurityKey jwtKey;

        public JWTService(IConfiguration configuration)
        {
            this.configuration = configuration;

            // lấy key => chuyển sang mảng bytes => tiến hành mã hóa đối xứng
            jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
        }

        public string CreateJWT(AppUser user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!)
            };

            // thông tin đăng nhập
            var creadentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(configuration["JWT:ExpiresInDays"]!)),
                SigningCredentials = creadentials,
                Issuer = configuration["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
