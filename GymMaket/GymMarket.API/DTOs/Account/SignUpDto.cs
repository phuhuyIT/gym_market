using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class SignUpDto
    {
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tên đầy đủ")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "{0} sai định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mật khẩu")]
        [StringLength(maximumLength: 16, MinimumLength = 8, ErrorMessage = "{0} ít nhất {2} và tối đa {1} ký tự")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;
    }
}
