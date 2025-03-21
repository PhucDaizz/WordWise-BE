using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Transactions;
using WordWise.Api.Models.Domain;
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
            try
            {
                var flashcardSet = await _flashcardSetRepository.GetAsync(flashcardSetId);
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

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            var flashcardSet = await _flashcardSetService.DeleteByIdAsync(id, userId);
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


    }
}
