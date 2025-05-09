using WordWise.Api.Models.Dto.UserLearningstats;

namespace WordWise.Api.Services.Interface
{
    public interface IUserLearningStatsService
    {
        Task<SessionResultDto> StartSessionAsync(string userId);
        Task<SessionResultDto> EndSessionAsync(string userId);
        Task<int> UpdateStreakAsync(string userId);
        Task<Statistics> GetStatisticsAsync();
    }
}
