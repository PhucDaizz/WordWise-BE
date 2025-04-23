using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashcardReview;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashcardReviewController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly WordWiseDbContext dbContext;
        private readonly IFlashcardReviewRepository _flashcardReviewRepository;

        public FlashcardReviewController(IMapper mapper, WordWiseDbContext dbContext, IFlashcardReviewRepository flashcardReviewRepository)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            _flashcardReviewRepository = flashcardReviewRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("review/{flashcardSetId:Guid}")]
        public async Task<IActionResult> CreateReview([FromRoute] Guid flashcardSetId, [FromBody] CreateFlashcardReviewDto createFlashcardReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Please, login again to post this review");
            }

            try
            {
                var review = mapper.Map<FlashcardReview>(createFlashcardReviewDto);
                review.UserId = userId;
                review.FlashcardSetId = flashcardSetId;
                review.CreateAt = DateTime.UtcNow;

                var result = await _flashcardReviewRepository.CreateAsync(review);

                if (result == null)
                {
                    return BadRequest("You can only post one review per flashcard set.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }


        [Authorize]
        [HttpDelete]
        [Route("delete/{flashcardReviewId:Guid}")]
        public async Task<IActionResult> DeleteReviewById([FromRoute]Guid flashcardReviewId)
        {
            var userId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("SuperAdmin");

            if(string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Login again to remove this comment");
            }
            var review = await _flashcardReviewRepository.DeleteByIdAsync(flashcardReviewId, userId);
            if(review == null)
            {
                return NotFound("This review is not existing");
            }
            return Ok(review);
        }

        [Authorize]
        [HttpPut]
        [Route("update/{flashcardSetId:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid flashcardSetId, [FromBody]CreateFlashcardReviewDto createFlashcardReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Login again to update this comment");
            }
            var updateReview = mapper.Map<FlashcardReview>(createFlashcardReviewDto);
            updateReview.UserId = userId;
            updateReview.FlashcardSetId = flashcardSetId;
            updateReview.CreateAt = DateTime.UtcNow;

            var updatedReview = await _flashcardReviewRepository.UpdateAsync(updateReview);

            if (updatedReview == null)
            {
                return NotFound("Review not found or you do not have permission to update it.");
            }

            return Ok(updatedReview);

        }
    }
}
