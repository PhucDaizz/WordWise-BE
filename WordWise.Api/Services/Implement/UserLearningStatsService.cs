using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.UserLearningstats;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class UserLearningStatsService : IUserLearningStatsService
    {
        private readonly IUserLearningStatsRepository _repository;
        private readonly WordWiseDbContext _dbContext;

        public UserLearningStatsService(IUserLearningStatsRepository repository, WordWiseDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<SessionResultDto> EndSessionAsync(string userId)
        {
            var stats = await _repository.GetByUserIdAsync(userId)
                   ?? throw new KeyNotFoundException($"User {userId} not found");

            if (stats.SessionStartTime == null)
            {
                throw new InvalidOperationException($"No active session for user {userId}");
            }

            var endTime = DateTime.UtcNow;
            var duration = endTime - stats.SessionStartTime.Value;

            stats.TotalLearningMinutes += duration.TotalMinutes;
            stats.SessionEndTime = endTime;
            stats.SessionStartTime = null;

            await _repository.UpdateAsync(stats);

            return new SessionResultDto
            {
                TimeStart = stats.SessionStartTime,
                TimeFinish = endTime,
                DurationMinutes = duration.TotalMinutes,
                CurrentStreak = stats.CurrentStreak,
                TotalLearningMinutes = stats.TotalLearningMinutes
            };
        }

        public async Task<Statistics> GetStatisticsAsync()
        {
            int totalUsers = await _dbContext.ExtendedIdentityUsers.CountAsync();
            int totalFlashCardSet = await _dbContext.FlashcardSets.CountAsync();
            int totalMultichoiceTest = await _dbContext.MultipleChoiceTests.CountAsync();
            int totalWritingTest = await _dbContext.WritingExercises.CountAsync();
            int totalReport = await _dbContext.ContentReports.CountAsync();

            double avgTotalLearningMinutes = await _dbContext.UserLearningStats.AverageAsync(x => x.TotalLearningMinutes);
            double avgCurrentStreak = await _dbContext.UserLearningStats.AverageAsync(x => x.CurrentStreak);

            return new Statistics
            {
                TotalUser = totalUsers,
                TotalFlashcardSet = totalFlashCardSet,
                TotalMultichoiceTest = totalMultichoiceTest,
                TotalWritingTest = totalWritingTest,
                TotalReport = totalReport,
                AverageTotalLearningMinutes = avgTotalLearningMinutes,
                AverageCurrentStreak = avgCurrentStreak
            };

        }

        public async Task<SessionResultDto> StartSessionAsync(string userId)
        {
            var stats = await _repository.GetByUserIdAsync(userId)
                   ?? throw new KeyNotFoundException($"User {userId} not found");

            if (stats.SessionStartTime != null)
            {
                throw new InvalidOperationException($"Session already started for user {userId}");
            }

            stats.SessionStartTime = DateTime.UtcNow;
            stats.SessionEndTime = null;

            await _repository.UpdateAsync(stats);

            return new SessionResultDto
            {
                TimeStart = stats.SessionStartTime,
                TimeFinish = null
            };
        }

        public async Task<int> UpdateStreakAsync(string userId)
        {
            var stats = await _repository.GetByUserIdAsync(userId)
                   ?? throw new KeyNotFoundException($"User {userId} not found");

            var today = DateTime.UtcNow.Date;

            // If new user
            if (stats.LastLearningDate == null)
            {
                stats.CurrentStreak = 1; 
                stats.LongestStreak = 1;
                stats.LastLearningDate = today; 
                await _repository.UpdateAsync(stats); 
                return stats.CurrentStreak; 
            }


            if (stats.LastLearningDate >= today)
            {
                return stats.CurrentStreak;
            }

            if (stats.LastLearningDate == today.AddDays(-1))
            {
                stats.CurrentStreak++;
                stats.LongestStreak = Math.Max(stats.LongestStreak, stats.CurrentStreak);
            }
            else if (stats.LastLearningDate < today.AddDays(-1))
            {
                stats.CurrentStreak = 1;
            }

            stats.LastLearningDate = today;
            await _repository.UpdateAsync(stats);

            return stats.CurrentStreak;
        }
    }
}
