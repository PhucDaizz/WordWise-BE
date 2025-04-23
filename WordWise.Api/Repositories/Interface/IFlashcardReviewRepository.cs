using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IFlashcardReviewRepository
    {
        Task<FlashcardReview?> CreateAsync(FlashcardReview flashcardReview);

        Task<FlashcardReview?> DeleteByIdAsync(Guid flashcardReviewId, string userId, bool isAdmin = false);

        Task<bool> DeleteAllReviewByFlashcardSetIdAsync(Guid flashcardSetId, string userId);

        Task<FlashcardReview?> UpdateAsync(FlashcardReview flashcardReview);
    }
}
