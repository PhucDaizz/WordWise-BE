using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.User;

namespace WordWise.Api.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateToken(ExtendedIdentityUser identityUser, List<string> roles);
        Task<LoginResponseDto> Login(LoginDto user);
        Task<LoginResponseDto> RefreshToken(RefreshTokenModel refreshToken);
    }
}
