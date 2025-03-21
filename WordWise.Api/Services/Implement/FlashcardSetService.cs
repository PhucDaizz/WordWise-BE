using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCardSet;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class FlashcardSetService : IFlashcardSetService
    {
        private readonly WordWiseDbContext dbContext;
        private readonly IFlashcardSetRepository _flashcardSetRepository;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly IFlashcardReviewRepository _flashcardReviewRepository;

        public FlashcardSetService(WordWiseDbContext dbContext, IFlashcardSetRepository flashcardSetRepository, IFlashCardRepository flashCardRepository, IFlashcardReviewRepository flashcardReviewRepository)
        {
            this.dbContext = dbContext;
            _flashcardSetRepository = flashcardSetRepository;
            _flashCardRepository = flashCardRepository;
            _flashcardReviewRepository = flashcardReviewRepository;
        }
        public async Task<FlashcardSet?> DeleteByIdAsync(Guid id, string userId)
        {
            await using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Delete flcardSet and flcards
                var review = await _flashcardReviewRepository.DeleteAllReviewByFlashcardSetIdAsync(id, userId);
                if (review)
                {
                    var flcardSet = await _flashcardSetRepository.DeleteAsync(id, userId);
                    // Delete Review

                    await trans.CommitAsync();
                    return flcardSet;
                }
                
                await trans.CommitAsync();
                return null;
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }
    }
}
