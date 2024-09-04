using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;
    }
}
