using WordWise.Api.Models.Dto.UserLearningstats;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class UserLearningStatsService : IUserLearningStatsService
    {
        private readonly IUserLearningStatsRepository _repository;

        public UserLearningStatsService(IUserLearningStatsRepository repository)
        {
            _repository = repository;
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
