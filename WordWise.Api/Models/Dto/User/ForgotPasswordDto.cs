using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.User
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? ClientUrl { get; set; }
    }
}
