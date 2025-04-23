using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class FlashcardReviewRepository : IFlashcardReviewRepository
    {
        private readonly WordWiseDbContext dbContext;

        public FlashcardReviewRepository(WordWiseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<FlashcardReview?> CreateAsync(FlashcardReview flashcardReview)
        {
            var findReview = await dbContext.FlashcardReviews
                .FirstOrDefaultAsync(x => x.FlashcardSetId == flashcardReview.FlashcardSetId && x.UserId == flashcardReview.UserId);

            if (findReview != null)
            {
                return null;
            }

            var review = await dbContext.FlashcardReviews.AddAsync(flashcardReview);

            await dbContext.SaveChangesAsync();

            return review.Entity;
        }


        public async Task<bool> DeleteAllReviewByFlashcardSetIdAsync(Guid flashcardSetId, string userId)
        {
            var flashcardSet = await dbContext.FlashcardSets
                .Include(x => x.FlashcardReviews)
                .FirstOrDefaultAsync(x => x.FlashcardSetId == flashcardSetId && x.UserId == userId);

            dbContext.FlashcardReviews.RemoveRange(flashcardSet.FlashcardReviews);
            await dbContext.SaveChangesAsync();

            return true; 
        }

        public async Task<FlashcardReview?> DeleteByIdAsync(Guid flashcardReviewId, string userId, bool isAdmin = false)
        {
            var existing = dbContext.FlashcardReviews.FirstOrDefault(x => x.FlashcardReviewId == flashcardReviewId);

            if (isAdmin)
            {
                userId = existing.UserId;
            }

            if (existing == null || userId != existing.UserId)
            {
                return null;
            }
            dbContext.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<FlashcardReview?> UpdateAsync(FlashcardReview flashcardReview)
        {
            var existing = await dbContext.FlashcardReviews.FirstOrDefaultAsync(x => x.FlashcardSetId == flashcardReview.FlashcardSetId && x.UserId == flashcardReview.UserId);
            if(existing == null)
            {
                return null;
            }

            flashcardReview.FlashcardReviewId = existing.FlashcardReviewId;
            flashcardReview.CreateAt = DateTime.UtcNow;

            dbContext.FlashcardReviews.Entry(existing).CurrentValues.SetValues(flashcardReview);
            await dbContext.SaveChangesAsync();

            return existing;
        }
    }
}
