using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymMarket.API.Services
{
    public class JWTService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly GymMarketContext _context;
        private readonly SymmetricSecurityKey _jwtKey;

        public JWTService(IConfiguration configuration, UserManager<AppUser> userManager, GymMarketContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;

            // lấy key => chuyển sang mảng bytes => tiến hành mã hóa đối xứng
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        }

        public async Task<string> CreateJWT(AppUser user)
        {
            var trainerId = await _context.Trainers.AsNoTracking().Where(t => t.UserId == user.Id).Select(t => t.TrainerId).FirstOrDefaultAsync();
            var studentId = await _context.Students.AsNoTracking().Where(t => t.UserId == user.Id).Select(t => t.StudentId).FirstOrDefaultAsync();
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.HomePhone, string.IsNullOrEmpty(user.PhoneNumber) == false ? user.PhoneNumber : ""),
                new Claim("trainerId", string.IsNullOrEmpty(trainerId) == false ? trainerId : ""),
                new Claim("studentId", string.IsNullOrEmpty(studentId) == false ? studentId : ""),
                new Claim("avatar", string.IsNullOrEmpty(user.Avatar) == false ? user.Avatar : "https://cdn-icons-png.flaticon.com/512/1999/1999625.png"),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // thông tin đăng nhập
            var creadentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:ExpiresInDays"]!)),
                SigningCredentials = creadentials,
                Issuer = _configuration["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
