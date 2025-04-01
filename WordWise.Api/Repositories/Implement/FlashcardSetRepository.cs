using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCardSet;
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

        public async Task<bool> ChangeStatusAsync(Guid flashcardSetId, string userId)
        {
            var existing = await dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSetId);
            if (existing == null || existing.UserId != userId)
            {
                return false;
            }
            existing.IsPublic = !existing.IsPublic;

            dbContext.FlashcardSets.Update(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<FlashcardSet?> CreateAsync(FlashcardSet flashcardSet)
        {
            // Litmit create 5 flcSet for 1 user
            var isLimitReached = await dbContext.FlashcardSets
                .Where(x => x.UserId == flashcardSet.UserId)
                .Skip(4) 
                .AnyAsync();

            if (isLimitReached)
            {
                return null; 
            }

            var flashcardSetEntry = await dbContext.FlashcardSets.AddAsync(flashcardSet);
            await dbContext.SaveChangesAsync();
            return flashcardSetEntry.Entity;
        }

        public async Task<FlashcardSet?> DeleteAsync(Guid flashcardSetId, string userId)
        {
            var flashcardSet = await dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSetId);
            if (flashcardSet == null || flashcardSet.UserId != userId)
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
            var flashcardSet = dbContext.FlashcardSets
                .Include(x => x.Flashcards)
                .Include(x => x.User)
                .Include(x => x.FlashcardReviews)
                .FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSetId);
            return flashcardSet;
        }

        public async Task<GetAllFlashCardSetDto> GetPublicAsync(string? learningLanguage, string? nativeLanguage, int currentPage = 1, int itemPerPage = 20)
        {
            if (currentPage <= 0 || itemPerPage <= 0)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }
            var query = dbContext.FlashcardSets
                .Where(x => x.IsPublic == true)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(learningLanguage))
            {
                query = query.Where(x => x.LearningLanguage == learningLanguage);
            }

            if (!string.IsNullOrWhiteSpace(nativeLanguage))
            {
                query = query.Where(x => x.NativeLanguage == nativeLanguage);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemPerPage);

            var items = await query
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .Select(x => new FlashcardSetSummaryDto
                {
                    FlashcardSetId = x.FlashcardSetId,
                    Title = x.Title,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    LearningLanguage = x.LearningLanguage,
                    NativeLanguage = x.NativeLanguage
                })
                .ToListAsync();
            return new GetAllFlashCardSetDto
            {
                FlashcardSets = items,
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }

        public async Task<GetAllFlashCardSetDto?> GetSummaryAsync(string userIdFind, string? yourUserId, int currentPage = 1, int itemPerPage = 5)
        {
            var isOwner = userIdFind == yourUserId;
            if (userIdFind == null || string.IsNullOrEmpty(userIdFind))
            {
                return null;
            }

            // Query FlashCardSets
            var query = dbContext.FlashcardSets
               .Where(x => x.UserId == userIdFind && (isOwner || x.IsPublic))
               .OrderByDescending(x => x.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemPerPage);

            // Paging
            var items = await query
               .Skip((currentPage - 1) * itemPerPage)
               .Take(itemPerPage)
               .Select(x => new FlashcardSetSummaryDto
               {
                   FlashcardSetId = x.FlashcardSetId,
                   Title = x.Title,
                   Description = x.Description,
                   CreatedAt = x.CreatedAt,
                   LearningLanguage = x.LearningLanguage,
                   NativeLanguage = x.NativeLanguage
               })
               .ToListAsync();

            // get username
            var userName = await dbContext.ExtendedIdentityUsers
                .Where(x => x.Id == userIdFind)
                .Select(x => x.UserName)
                .FirstOrDefaultAsync();

            return new GetAllFlashCardSetDto
            {
                FlashcardSets = items,
                UserName = userName ?? "Unknown",
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }

        public async Task<FlashcardSet?> UpdateAsync(FlashcardSet flashcardSet)
        {
            var existing = await dbContext.FlashcardSets.FirstOrDefaultAsync(fs => fs.FlashcardSetId == flashcardSet.FlashcardSetId);
            if (existing == null || existing.UserId != flashcardSet.UserId)
            {
                return null;
            }

            flashcardSet.CreatedAt = existing.CreatedAt;
            flashcardSet.IsPublic = existing.IsPublic;

            dbContext.Entry(existing).CurrentValues.SetValues(flashcardSet);
            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
