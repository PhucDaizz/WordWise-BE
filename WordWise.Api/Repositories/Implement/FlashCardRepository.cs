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
            // limit 50 flashcard for 1 flahcardSet
            var isLimitReached = await dbContext.Flashcards
                                    .Where(x => x.FlashcardSetId == flashcard.FlashcardSetId)
                                    .Skip(49)
                                    .AnyAsync();
            if (isLimitReached)
            {
                return null;
            }

            var flashCard = await dbContext.Flashcards.AddAsync(flashcard);
            await dbContext.SaveChangesAsync();
            return flashCard.Entity;
        }

        public async Task<int> CreateRangeAsync(Guid flashcardSetId, ICollection<Flashcard> flashcards)
        {

            var countFlashcard = await dbContext.Flashcards
                .Where(x => x.FlashcardSetId == flashcardSetId)
                .CountAsync();

            var maxToAdd = Math.Max(0, 50 - countFlashcard);

            if (maxToAdd == 0)
            {
                return 0;
            }

            var flashcardsToAdd = flashcards.Take(maxToAdd).ToList();

            await dbContext.Flashcards.AddRangeAsync(flashcardsToAdd);
            await dbContext.SaveChangesAsync();

            return flashcardsToAdd.Count;
        }

        public async Task<bool> DeleteAllFlashCardAync(Guid flashcardSetId)
        {
            try
            {
                var rowsAffected = await dbContext.Flashcards
                    .Where(x => x.FlashcardSetId == flashcardSetId)
                    .ExecuteDeleteAsync();

                return rowsAffected > 0; 
            }
            catch (Exception ex)
            {
                return false; 
            }
        }


        public async Task<Flashcard?> DeleteAsync(int flashCardId, string userId)
        {
            var existing = await dbContext.Flashcards
                .Where(f => f.FlashcardId == flashCardId)
                .Select(f => new
                {
                    Flashcard = f,
                    UserId = f.FlashcardSet.UserId
                })
                .FirstOrDefaultAsync();

            if (existing == null || existing.UserId != userId)
            {
                return null;
            }

            dbContext.Flashcards.Remove(existing.Flashcard);
            await dbContext.SaveChangesAsync();

            return existing.Flashcard;
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

        public async Task<Flashcard?> UpdateAsync(Flashcard flashcard, string userId)
        {
            
            var existing = await dbContext.Flashcards.FirstOrDefaultAsync(f => f.FlashcardId == flashcard.FlashcardId);
            if (existing == null)
            {
                return null;
            }
            if(!await CheckUserPermissions(userId, existing.FlashcardSetId))
            {
                return null;
            }

            flashcard.FlashcardSetId = existing.FlashcardSetId;
            flashcard.CreatedAt = DateTime.UtcNow;
            dbContext.Entry(existing).CurrentValues.SetValues(flashcard);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> CheckUserPermissions(string userId, Guid flashcardSetId)
        {
            var flashcardSet = await dbContext.FlashcardSets
                .FirstOrDefaultAsync(x => x.FlashcardSetId == flashcardSetId);

            if (flashcardSet == null)
            {
                return false; 
            }

            if (flashcardSet.UserId != userId)
            {
                return false; 
            }

            return true; 
        }


    }
}
