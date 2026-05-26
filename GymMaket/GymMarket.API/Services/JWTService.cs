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

            // Get key => convert to bytes array => perform symmetric encryption
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        }

        public async Task<string> CreateJWT(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            string? trainerId = null;
            string? studentId = null;

            if (userRoles.Contains(ApplicationRoles.Trainer))
            {
                var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (trainer == null)
                {
                    trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);
                    if (trainer != null)
                    {
                        trainer.UserId = user.Id;
                        _context.Trainers.Update(trainer);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        trainer = new Trainer
                        {
                            TrainerId = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            Name = user.FullName,
                            Email = user.Email,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            ProfilePicture = Defaults.AvatarUrl,
                            Certification = "General Fitness Trainer",
                            Bio = "Professional fitness instructor.",
                            Experience = 0,
                            Rating = Defaults.DefaultRating,
                            Description = ""
                        };
                        await _context.Trainers.AddAsync(trainer);
                        await _context.SaveChangesAsync();
                    }
                }
                trainerId = trainer.TrainerId;
            }
            else if (userRoles.Contains(ApplicationRoles.Student))
            {
                var student = await _context.Students.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (student == null)
                {
                    student = await _context.Students.FirstOrDefaultAsync(t => t.Email == user.Email);
                    if (student != null)
                    {
                        student.UserId = user.Id;
                        _context.Students.Update(student);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        student = new Student
                        {
                            StudentId = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            Name = user.FullName,
                            Email = user.Email,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            HealthStatus = "Good",
                            ProfilePicture = Defaults.StudentAvatarUrl
                        };
                        await _context.Students.AddAsync(student);
                        await _context.SaveChangesAsync();
                    }
                }
                studentId = student.StudentId;
            }
            else
            {
                trainerId = await _context.Trainers.AsNoTracking().Where(t => t.UserId == user.Id).Select(t => t.TrainerId).FirstOrDefaultAsync();
                studentId = await _context.Students.AsNoTracking().Where(t => t.UserId == user.Id).Select(t => t.StudentId).FirstOrDefaultAsync();
            }

            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.HomePhone, string.IsNullOrEmpty(user.PhoneNumber) == false ? user.PhoneNumber : ""),
                new Claim("trainerId", string.IsNullOrEmpty(trainerId) == false ? trainerId : ""),
                new Claim("studentId", string.IsNullOrEmpty(studentId) == false ? studentId : ""),
                new Claim("avatar", string.IsNullOrEmpty(user.Avatar) == false ? user.Avatar : Defaults.AvatarUrl),
            };

            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Signing credentials
            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:ExpiresInDays"]!)),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
