using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.UserLearningstats;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class UserLearningStatsRepository: IUserLearningStatsRepository
    {
        private readonly WordWiseDbContext dbContext;

        public UserLearningStatsRepository(WordWiseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserLearningStats> CreateAsync(UserLearningStats userLearningStats)
        {
            await dbContext.UserLearningStats.AddAsync(userLearningStats);
            await dbContext.SaveChangesAsync();
            return userLearningStats;
        }

        public async Task<UserLearningStats> GetByUserIdAsync(string userId)
        {
            return await dbContext.UserLearningStats
            .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task UpdateAsync(UserLearningStats stats)
        {
            dbContext.UserLearningStats.Update(stats);
            await dbContext.SaveChangesAsync();
        }

    }
    
}
