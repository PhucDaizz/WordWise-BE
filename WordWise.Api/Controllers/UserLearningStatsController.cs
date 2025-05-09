using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLearningStatsController : ControllerBase
    {
        private readonly IUserLearningStatsRepository _userLearningStatsRepository;
        private readonly IUserLearningStatsService _userLearningStatsService;

        public UserLearningStatsController(IUserLearningStatsRepository userLearningStatsRepository, IUserLearningStatsService userLearningStatsService)
        {
            _userLearningStatsRepository = userLearningStatsRepository;
            _userLearningStatsService = userLearningStatsService;
        }



        [HttpPost]
        [Route("UpdateStreak")]
        [Authorize]
        public async Task<IActionResult> UpdateSteak()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userLearningStatsService.UpdateStreakAsync(userId);
                return Ok(result);

            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the streak.");
            }

        }

        [Authorize]
        [HttpPost]
        [Route("StartLearn")]
        public async Task<IActionResult> StartLearningSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userLearningStatsService.StartSessionAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while starting the learning session.");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("EndLearn")]
        public async Task<IActionResult> EndLearningSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userLearningStatsService.EndSessionAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while ending the learning session.");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetReport")]
        public async Task<IActionResult> GetReport()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userLearningStatsRepository.GetByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the learning report.");
            }
        }


        [Authorize(Roles ="Admin, SuperAdmin")]
        [HttpGet]
        [Route("Statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var result = await _userLearningStatsService.GetStatisticsAsync();
                return Ok(result);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the statistics.");
            }
        }



    }
}
