using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.User;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ExtendedIdentityUser> userManager;

        public TokenRepository(IConfiguration configuration, UserManager<ExtendedIdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public string CreateToken(ExtendedIdentityUser identityUser, List<string> roles)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim(ClaimTypes.Email, identityUser.Email),
                new Claim(ClaimTypes.Name, identityUser.UserName)
            };
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claim,
                expires: DateTime.Now.AddMinutes(200),
                signingCredentials: creds
            );
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDto> Login(LoginDto user)
        {
            var response = new LoginResponseDto();
            var identityUser = await userManager.FindByEmailAsync(user.Email);
            if (identityUser == null || (await userManager.CheckPasswordAsync(identityUser, user.Password)) == false)
            {
                return response;
            }

            response.Email = identityUser.Email;
            response.Roles = await userManager.GetRolesAsync(identityUser);
            response.Token = CreateToken(identityUser, response.Roles.ToList());
            response.RefreshToken = Guid.NewGuid().ToString();

            identityUser.RefreshToken = response.RefreshToken;
            identityUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(identityUser);
            return response;
        }

        public async Task<LoginResponseDto> RefreshToken(RefreshTokenModel refreshToken)
        {
            var principal = GetTokenPrincipal(refreshToken.Token);

            var response = new LoginResponseDto();
            var email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return response;
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != refreshToken.RefreshToken || user.RefreshTokenExpiry > DateTime.UtcNow)
            {
                return response;
            }
            response.Email = email;
            response.Roles = await userManager.GetRolesAsync(user);
            response.Token = CreateToken(user, response.Roles.ToList());
            response.RefreshToken = GenerateRefreshToken();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);
            return response;
        }

        private ClaimsPrincipal GetTokenPrincipal(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = securityKey
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var validatedToken);
        }

        private string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            return Convert.ToBase64String(randomnumber);
        }
    }
}
