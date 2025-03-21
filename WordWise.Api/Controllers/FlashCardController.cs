using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCard;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashCardController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly ILogger<FlashCardController> logger;

        public FlashCardController(IMapper mapper, IFlashCardRepository flashCardRepository, ILogger<FlashCardController> logger)
        {
            this.mapper = mapper;
            _flashCardRepository = flashCardRepository;
            this.logger = logger;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateFlashCard([FromBody] CreateFlashCard flashCard)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if(userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                if (!await _flashCardRepository.CheckUserPermissions(userId, flashCard.FlashcardSetId))
                {
                    return Forbid("You do not have permission to create flashcards in this set.");
                }

                if (flashCard == null)
                {
                    return BadRequest("FlashCard data is required.");
                }

                var flCard = mapper.Map<Flashcard>(flashCard);
                flCard.CreatedAt = DateTime.UtcNow;

                var result = await _flashCardRepository.CreateAsync(flCard);

                if(result == null)
                {
                    return BadRequest("You have reached the maximum limit of 50 flashcard.");
                }

                var flCardDto = mapper.Map<FlashCardDto>(result);

                return Ok(flCardDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateFlashCard");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the flashcard.");
            }
        }

        [Authorize]
        [HttpPut("Update/{flashCardId:int}")]
        public async Task<IActionResult> UpdateFlashCard([FromRoute]int flashCardId ,[FromBody] UpdateFlashCard flashCard)
        {
            try
            {

                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                if (flashCard == null)
                {
                    return BadRequest("FlashCard data is required.");
                }

                var flCard = mapper.Map<Flashcard>(flashCard);
                flCard.FlashcardId = flashCardId;
                
                var result = await _flashCardRepository.UpdateAsync(flCard, userId);

                if (result == null)
                {
                    return NotFound("FlashCard not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the flashcard.");
            }
        }

        [Authorize]
        [HttpDelete("Delete/{flashCardId:int}")]
        public async Task<IActionResult> DeleteFlashCard([FromRoute]int flashCardId)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null)
                {
                    return Unauthorized("Please login again!");
                }
                    
                var result = await _flashCardRepository.DeleteAsync(flashCardId, userId);

                if (result == null)
                {
                    return NotFound("FlashCard not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the flashcard.");
            }
        }

        [HttpGet("GetAllByFlashcardSetId/{flashcardSetId:Guid}")]
        public async Task<IActionResult> GetAllByFlashcardSetId([FromRoute]Guid flashcardSetId)
        {
            try
            {
                var result = await _flashCardRepository.GetAllByFlashcardSetIdAsync(flashcardSetId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching flashcards.");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("CreateRange/{flashcardSetId:Guid}")]
        public async Task<IActionResult> CreateRange([FromRoute]Guid flashcardSetId, [FromBody] IEnumerable<CreateRangeFlashcardDto> createRangeFlashcardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Definition or Term, Example is too long");
            }

            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                if (!await _flashCardRepository.CheckUserPermissions(userId, flashcardSetId))
                {
                    return Forbid("You do not have permission to create flashcards in this set.");
                }

                var flashcards = mapper.Map<IEnumerable<Flashcard>>(createRangeFlashcardDto)
                    .Select(flashcard => {
                        flashcard.FlashcardSetId = flashcardSetId;
                        flashcard.CreatedAt = DateTime.UtcNow;
                        return flashcard;
                    }).ToList();
                var createdCount = await _flashCardRepository.CreateRangeAsync(flashcardSetId, flashcards);
                return CreatedAtRoute(null, new
                {
                    message = $"{createdCount} flashcards have been created.",
                    createdCount = createdCount
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the flashcard.");
            }

        }
    }
}
