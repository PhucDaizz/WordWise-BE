using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.User
{
    public class RefreshTokenModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
