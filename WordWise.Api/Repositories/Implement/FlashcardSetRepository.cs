using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class FlashcardSetRepository: IFlashcardSetRepository
    {
        private readonly WordWiseDbContext dbContext;

        public FlashcardSetRepository(WordWiseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FlashcardSet> CreateAsync(FlashcardSet flashcardSet)
        {
            var flashCardSet = await dbContext.FlashcardSets.AddAsync(flashcardSet);
            await dbContext.SaveChangesAsync();
            return flashCardSet.Entity;
        }

        public async Task<FlashcardSet?> DeleteAsync(Guid flashcardSetId)
        {
            var flashcardSet = await dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSetId);
            if (flashcardSet == null)
            {
                return null;
            }
            dbContext.FlashcardSets.Remove(flashcardSet);
            await dbContext.SaveChangesAsync();
            return flashcardSet;
        }

        public async Task<IEnumerable<FlashcardSet>> GetAllByUserIdAsync(string userId)
        {
            var flashcardSets = await dbContext.FlashcardSets.Where(fs => fs.UserId == userId).ToListAsync();
            return flashcardSets;
        }

        public Task<FlashcardSet?> GetAsync(Guid flashcardSetId)
        {
            var flashcardSet = dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSetId);
            if (flashcardSet == null)
            {
                return null;
            }
            return flashcardSet;
        }

        public async Task<FlashcardSet?> UpdateAsync(FlashcardSet flashcardSet)
        {
            var existing = await dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSet.FlashcardSetId);
            if (existing == null)
            {
                return null;
            }
            dbContext.Entry(existing).CurrentValues.SetValues(flashcardSet);
            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
