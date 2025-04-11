using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.User
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
