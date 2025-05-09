using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Transactions;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.AI;
using WordWise.Api.Models.Dto.FlashCardSet;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashCardSetController : ControllerBase
    {
        private readonly IFlashcardSetRepository _flashcardSetRepository;
        private readonly IMapper mapper;
        private readonly IFlashcardSetService _flashcardSetService;

        public FlashCardSetController(IFlashcardSetRepository flashcardSetRepository, IMapper mapper, IFlashcardSetService flashcardSetService)
        {
            _flashcardSetRepository = flashcardSetRepository;
            this.mapper = mapper;
            _flashcardSetService = flashcardSetService;
        }


        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> CreateFlashCardSet([FromBody] CreateFlashCardSetDto createFlashCardSetDto)
        {
            try
            {
                var flashcardSet = mapper.Map<FlashcardSet>(createFlashCardSetDto);

                // Get UserId
                var userId = HttpContext.User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                flashcardSet.UserId = userId;
                flashcardSet.CreatedAt = DateTime.UtcNow;
                flashcardSet.UpdatedAt = DateTime.UtcNow;
                var createFlashcardSet = await _flashcardSetRepository.CreateAsync(flashcardSet);
                if(createFlashcardSet != null)
                {
                    return Ok(createFlashcardSet);
                }
                return BadRequest("You have reached the maximum limit of 5 flashcard sets.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred." + ex.Message);
            }
        }

        [HttpGet]
        [Route("{flashcardSetId:Guid}")]
        public async Task<IActionResult> GetFlashCardSet([FromRoute]Guid flashcardSetId)
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("SuperAdmin");

            try
            {
                var flashcardSet = await _flashcardSetService.GetByIdAsync(flashcardSetId, userId, isAdmin);
                if (flashcardSet == null)
                {
                    return NotFound("FlashcardSet is not found!");
                }
                var result = mapper.Map<FlashCardSetDto>(flashcardSet);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred." + ex.Message);
            }
            
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete/{id:Guid}")]
        public async Task<IActionResult> DeleteById([FromRoute]Guid id)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var isAdmin = HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("SuperAdmin");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            var flashcardSet = await _flashcardSetService.DeleteByIdAsync(id, userId, isAdmin);
            if (flashcardSet == null)
            {
                return NotFound("FlashCartSet is not existing");
            }
            return Ok(flashcardSet);
        }

        [HttpPut]
        [Authorize]
        [Route("Update/{flashcardSetId:Guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid flashcardSetId, [FromBody]UpdateFlashcardSet updateFlashcardSet)
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Please login again!");
            }

            var flashcardSet = mapper.Map<FlashcardSet>(updateFlashcardSet);

            flashcardSet.FlashcardSetId = flashcardSetId;
            flashcardSet.UserId = userId;
            flashcardSet.UpdatedAt= DateTime.UtcNow;

            var result = await _flashcardSetRepository.UpdateAsync(flashcardSet);

            if(result == null) {
                return NotFound("FlashcardSet id not existing");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [Route("Topublic/{flashcardSetId:Guid}")]
        public async Task<IActionResult> ToPublic([FromRoute] Guid flashcardSetId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Please login again!");
            }

            var result = await _flashcardSetRepository.ChangeStatusAsync(flashcardSetId, userId);

            if (!result)
            {
                return BadRequest("FlashcardSet not found or you do not have permission to change its status.");
            }

            return Ok("The status of the FlashcardSet has been successfully changed.");
        }


        [HttpGet]
        [Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery] string? learningLanguage, [FromQuery] string? nativeLanguage, [FromQuery] int page = 1, [FromQuery] int itemPerPage = 20)
        {
            if (page <= 0 || itemPerPage <= 0)
            {
                return BadRequest("Page and items per page must be greater than 0.");
            }
            try
            {
                var result = await _flashcardSetRepository.GetPublicAsync(learningLanguage, nativeLanguage, page, itemPerPage);
                if (result == null || result.FlashcardSets == null || !result.FlashcardSets.Any())
                {
                    return NotFound("No data found.");
                }
                return Ok(result);
            }
            catch( ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("GetAll/{userId}")]
        public async Task<IActionResult> GetAll([FromRoute] string userId, [FromQuery] int page = 1, [FromQuery] int itemPerPage = 5)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is required.");
            }
            var userIdQuery = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _flashcardSetRepository.GetSummaryAsync(userId, userIdQuery, page, itemPerPage);

            if (result == null)
            {
                return NotFound("No data found.");
            }

            return Ok(result);
        }


        [HttpPost]
        [Authorize]
        [Route("GenerateByAI")]
        public async Task<IActionResult> GenerateByAI([FromBody]GenerateFlashcardSetRequest generateFlashcardSet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }


            try{
                var flashcardSet = mapper.Map<FlashcardSet>(generateFlashcardSet);
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                flashcardSet.UserId = userId;
                flashcardSet.CreatedAt = DateTime.UtcNow;
                flashcardSet.UpdatedAt = DateTime.UtcNow;
                flashcardSet.Description = "Auto generated flashcard set";
                flashcardSet.IsPublic = true;

                var result = await _flashcardSetService.AutoGenerateByAi(flashcardSet);

                var flashcardSetDto = mapper.Map<FlashcardSetSummaryDto>(result);

                return Ok(flashcardSetDto);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("API Key not found"))
                {
                    return BadRequest(new { code = "API_KEY_NOT_FOUND", message = e.Message });
                }
                else if (e.Message.Contains("maximum limit"))
                {
                    return BadRequest(new { code = "LIMIT_REACHED", message = e.Message });
                }
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }   
        }


        [HttpGet]
        [Route("admin/GetAllAdmin")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllAdmin([FromQuery]Guid? flashCardSetId, [FromQuery]string? learningLanguage, [FromQuery]string? nativeLanguage, [FromQuery]int page = 1, [FromQuery]int itemPerPage = 20)
        {
            try
            {
                var result = await _flashcardSetRepository.GetAllAdminAsync(flashCardSetId, learningLanguage, nativeLanguage, page, itemPerPage);
                return Ok(result);
            }
            catch (ArgumentException Ex)
            {
                return BadRequest(Ex);
            }

        }
    }
}
