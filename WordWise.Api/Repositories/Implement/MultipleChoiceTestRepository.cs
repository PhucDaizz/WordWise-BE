using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.MultipleChoiceTest;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class MultipleChoiceTestRepository : IMultipleChoiceTestRepository
    {
        private readonly WordWiseDbContext dbContext;
        private readonly IMapper mapper;

        public MultipleChoiceTestRepository(WordWiseDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public async Task<MultipleChoiceTest?> CreateAsync(MultipleChoiceTest multipleChoiceTest, bool isAI = false)
        {
            if (!isAI)
            {
                var tests = await dbContext.MultipleChoiceTests.Where(x => x.UserId == multipleChoiceTest.UserId).CountAsync();

                if (tests >= 5)
                {
                    return null;
                }
            }

            var result = await dbContext.MultipleChoiceTests.AddAsync(multipleChoiceTest);
            await dbContext.SaveChangesAsync();
            return result.Entity;

        }

        public async Task<bool> DeleteAsync(Guid multipleChoiceTestId, string userId)
        {
            var existing = await dbContext.MultipleChoiceTests.FirstOrDefaultAsync(x => x.UserId == userId && x.MultipleChoiceTestId == multipleChoiceTestId);
            if (existing == null)
            {
                return false;
            }
            dbContext.MultipleChoiceTests.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<MultipleChoiceTest?> GetByIdAsync(Guid id)
        {
            var existing = await dbContext.MultipleChoiceTests.Include(x => x.Questions).FirstOrDefaultAsync(x => x.MultipleChoiceTestId == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<GetAllMultipleChoiceTestDto> GetPublicAsync(string? learningLanguage, string? nativeLanguage, int currentPage = 1, int itemPerPage = 20)
        {
            if (currentPage <= 0 || itemPerPage <= 0)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }

            var query = dbContext.MultipleChoiceTests
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
                .Select(x => new MultipleChoiceTestSummaryDto
                {
                    MultipleChoiceTestId = x.MultipleChoiceTestId,
                    Title = x.Title,
                    CreateAt = x.CreateAt,
                    LearningLanguage = x.LearningLanguage,
                    NativeLanguage = x.NativeLanguage,
                    IsPublic = x.IsPublic
                })
                .ToListAsync();

            return new GetAllMultipleChoiceTestDto
            {
                multipleChoiceTestSummaries = items,
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }

        public async Task<GetAllMultipleChoiceTestDto?> GetSummaryAsync(string userIdFind, string? yourUserId, int currentPage = 1, int itemPerPage = 5)
        {
            var isOwner = userIdFind == yourUserId;
            if(userIdFind == null || string.IsNullOrEmpty(userIdFind))
            {
                return null;
            }

            // Query MultipleChoiceTests 
            var query = dbContext.MultipleChoiceTests
                .Where(x => x.UserId == userIdFind && (isOwner || x.IsPublic))
                .OrderByDescending(x => x.CreateAt); 

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemPerPage);

            // Paging
            var items = await query
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .Select(x => new MultipleChoiceTestSummaryDto
                {
                    MultipleChoiceTestId = x.MultipleChoiceTestId,
                    Title = x.Title,
                    CreateAt = x.CreateAt,
                    LearningLanguage = x.LearningLanguage,
                    NativeLanguage = x.NativeLanguage,
                    IsPublic = x.IsPublic
                })
                .ToListAsync();

            // get username
            var userName = await dbContext.ExtendedIdentityUsers
                .Where(x => x.Id == userIdFind)
                .Select(x => x.UserName)
                .FirstOrDefaultAsync();

            return new GetAllMultipleChoiceTestDto
            {
                multipleChoiceTestSummaries = items,
                UserName = userName ?? "Unknown",
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }

        public async Task<bool> SetStatusAsync(Guid multipleChoiceTestId, string userId)
        {
            var test = await dbContext.MultipleChoiceTests.FirstOrDefaultAsync(x => x.MultipleChoiceTestId == multipleChoiceTestId && x.UserId == userId);
            if (test == null)
            {
                return false;
            }
            test.IsPublic = !test.IsPublic;
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
