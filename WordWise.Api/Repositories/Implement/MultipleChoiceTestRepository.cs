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
        public async Task<MultipleChoiceTest?> CreateAsync(MultipleChoiceTest multipleChoiceTest)
        {
            var tests = await dbContext.MultipleChoiceTests.Where(x => x.UserId == multipleChoiceTest.UserId).CountAsync();

            if (tests >= 5)
            {
                return null;
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


    }
}
