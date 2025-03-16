using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class FlashCardRepository: IFlashCardRepository
    {
        private readonly WordWiseDbContext dbContext;

        public FlashCardRepository(WordWiseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Flashcard> CreateAsync(Flashcard flashcard)
        {
            var flashCard = await dbContext.Flashcards.AddAsync(flashcard);
            await dbContext.SaveChangesAsync();
            return flashCard.Entity;
        }

        public async Task<Flashcard?> DeleteAsync(int flashCardId)
        {
            var existing = await dbContext.Flashcards.FirstOrDefaultAsync(f => f.FlashcardId == flashCardId);
            if (existing == null)
            {
                return null;
            }
            dbContext.Flashcards.Remove(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<Flashcard>> GetAllByFlashcardSetIdAsync(Guid flashcardSetId)
        {
            var flashcards = await dbContext.Flashcards.Where(f => f.FlashcardSetId == flashcardSetId).ToListAsync();
            return flashcards;
        }

        public Task<Flashcard?> GetAsync(int flashCardId)
        {
            var flashcard = dbContext.Flashcards.FirstOrDefaultAsync(f => f.FlashcardId == flashCardId);
            if (flashcard == null)
            {
                return null;
            }
            return flashcard;
        }

        public async Task<Flashcard?> UpdateAsync(Flashcard flashcard)
        {
            var existing = await dbContext.Flashcards.FirstOrDefaultAsync(f => f.FlashcardId == flashcard.FlashcardId);
            if (existing == null)
            {
                return null;
            }
            flashcard.FlashcardSetId = existing.FlashcardSetId;
            dbContext.Entry(existing).CurrentValues.SetValues(flashcard);
            await dbContext.SaveChangesAsync();
            return existing;
        }


    }
}
