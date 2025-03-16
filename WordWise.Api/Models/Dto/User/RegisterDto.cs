using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.User
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        public LanguageLevel Level { get; set; }

    }
}
