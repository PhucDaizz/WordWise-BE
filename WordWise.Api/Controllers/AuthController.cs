using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.User;
using WordWise.Api.Repositories.Implement;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<ExtendedIdentityUser> userManager, IMapper mapper, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new ExtendedIdentityUser
            {
                UserName = registerDto.UserName?.Trim(),
                Email = registerDto.Email?.Trim(),
                Gender = registerDto.Gender,
                Level = registerDto.Level
            };

            var identityResult = await userManager.CreateAsync(user, registerDto.Password);
            if (identityResult.Succeeded)
            {
                identityResult = await userManager.AddToRoleAsync(user, "User");
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            var admin = new ExtendedIdentityUser
            {
                UserName = registerDto.UserName?.Trim(),
                Email = registerDto.Email?.Trim(),
                Gender = registerDto.Gender,
                Level = registerDto.Level
            };
            var identityResult = await userManager.CreateAsync(admin, registerDto.Password);
            if (identityResult.Succeeded)
            {
                List<string> roles = new List<string>() { "Admin", "User" };
                identityResult = await userManager.AddToRolesAsync(admin, roles);
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var loginResult = await tokenRepository.Login(loginDto);
            if (string.IsNullOrEmpty(loginResult.Token))
            {
                ModelState.AddModelError("", "Email or Password is not correct");
                return ValidationProblem(ModelState);
            }
            return Ok(loginResult);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var loginResult = await tokenRepository.RefreshToken(refreshToken);
            if (string.IsNullOrEmpty(loginResult.Token))
            {
                ModelState.AddModelError("", "Refresh token is not correct");
                return ValidationProblem(ModelState);
            }
            return Ok(loginResult);
        }

    }
}
