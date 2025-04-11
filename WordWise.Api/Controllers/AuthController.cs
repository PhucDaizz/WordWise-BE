using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.AI;
using WordWise.Api.Models.Dto.User;
using WordWise.Api.Repositories.Implement;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly ITokenRepository tokenRepository;
        private readonly ICacheService _cacheService;
        private readonly IUserLearningStatsRepository _userLearningStatsRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ExtendedIdentityUser> userManager, IMapper mapper, ITokenRepository tokenRepository, ICacheService cacheService, IUserLearningStatsRepository userLearningStatsRepository, IAuthRepository authRepository, IEmailService emailService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenRepository = tokenRepository;
            _cacheService = cacheService;
            _userLearningStatsRepository = userLearningStatsRepository;
            _authRepository = authRepository;
            _emailService = emailService;
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
                    // Create new UserLearningStats 
                    var userLearningStats = new UserLearningStats
                    {
                        UserId = user.Id,
                        CurrentStreak = 0,
                        LongestStreak = 0,
                        TotalLearningMinutes = 0,
                        LastLearningDate = null,
                        SessionStartTime = null,
                        SessionEndTime = null
                    };

                    // Save UserLearningStats to the database
                    await _userLearningStatsRepository.CreateAsync(userLearningStats);
                    await _emailService.SendEmailConfirmationAsync(user);
                    return Ok("User created successfully.");
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

        [Authorize]
        [HttpPost]
        [Route("store-key")]
        public async Task<IActionResult> StoreKey([FromBody] FillGeminiKeyDto fillGeminiKeyDto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Không thể lấy UserId từ token.");
                }

                if (string.IsNullOrEmpty(fillGeminiKeyDto.ApiKey))
                {
                    return BadRequest(new { Message = "GeminiApiKey không được để trống." });
                }

                // Kiểm tra xem API key đã tồn tại chưa
                if (_cacheService.TryGetApiKey(userId, out string existingApiKey))
                {
                    return Ok(new { Message = "API key đã tồn tại trong cache.", ApiKey = existingApiKey });
                }

                // Lưu API key mới
                _cacheService.StoreApiKey(userId, fillGeminiKeyDto.ApiKey);

                // Trả về thông báo lưu thành công
                return Ok(new { Message = "API key đã được lưu thành công vào cache.", ApiKey = fillGeminiKeyDto.ApiKey });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server.", Detail = ex.Message });
            }
        }


        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        [Route("get-all-user")]
        public async Task<IActionResult> GetAllUser([FromQuery] string? emailUser, [FromQuery] string? roleFilter, [FromQuery] int? page = 1, [FromQuery] int? itemPerPage = 20)
        {
            try
            {
                var listUser = await _authRepository.GetAllUserAsync(emailUser, roleFilter, page ?? 1, itemPerPage ?? 20);
                return Ok(listUser);
            }
            catch (ArgumentException e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                return BadRequest("No Accounts found with this email");
            }

            var decodedToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(resetPassword.Token));
            var result = await userManager.ResetPasswordAsync(user, decodedToken!, resetPassword.Password!);

            if (result.Succeeded)
            {
                return Ok("Password has been reset");
            }
            else
            {
                var erroes = result.Errors.Select(x => x.Description);
                return BadRequest(new { Errors = erroes });
            }
        }


        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return BadRequest("No Accounts found with this email");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            var param = new Dictionary<string, string>
            {
                {"token", encodedToken},
                {"email", forgotPasswordDTO.Email}
            };

            var callback = QueryHelpers.AddQueryString(forgotPasswordDTO.ClientUrl, param);

            await _emailService.SendPasswordResetEmailAsync(forgotPasswordDTO.Email, user.UserName, callback);

            return Ok("Reset password link has been sent to your email");
        }

        [Authorize]
        [HttpPost]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await _emailService.SendEmailConfirmationAsync(user);
            return Ok("Email has been sent");
        }






    }
}