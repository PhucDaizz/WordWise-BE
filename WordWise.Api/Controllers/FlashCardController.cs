using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IFlashCardRepository flashCardRepository;
        private readonly ILogger<FlashCardController> logger;

        public FlashCardController(IMapper mapper, IFlashCardRepository flashCardRepository, ILogger<FlashCardController> logger)
        {
            this.mapper = mapper;
            this.flashCardRepository = flashCardRepository;
            this.logger = logger;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateFlashCard([FromBody] CreateFlashCard flashCard)
        {
            try
            {
                if (flashCard == null)
                {
                    return BadRequest("FlashCard data is required.");
                }

                var flCard = mapper.Map<Flashcard>(flashCard);
                flCard.CreatedAt = DateTime.UtcNow;

                var result = await flashCardRepository.CreateAsync(flCard);

                return CreatedAtAction(nameof(CreateFlashCard), new { id = result.FlashcardId }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateFlashCard");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the flashcard.");
            }
        }

        [HttpPut("Update/{flashCardId:int}")]
        public async Task<IActionResult> UpdateFlashCard([FromRoute]int flashCardId ,[FromBody] UpdateFlashCard flashCard)
        {
            try
            {
                if (flashCard == null)
                {
                    return BadRequest("FlashCard data is required.");
                }

                var flCard = mapper.Map<Flashcard>(flashCard);

                var result = await flashCardRepository.UpdateAsync(flCard);

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

        [HttpDelete("Delete/{flashCardId:int}")]
        public async Task<IActionResult> DeleteFlashCard([FromRoute]int flashCardId)
        {
            try
            {
                var result = await flashCardRepository.DeleteAsync(flashCardId);

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

        [HttpGet("GetAllByFlashcardSetId/{flashcardSetId:guid}")]
        public async Task<IActionResult> GetAllByFlashcardSetId([FromRoute]Guid flashcardSetId)
        {
            try
            {
                var result = await flashCardRepository.GetAllByFlashcardSetIdAsync(flashcardSetId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching flashcards.");
            }
        }
    }
}
