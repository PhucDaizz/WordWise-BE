using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.UserLearningstats;

namespace WordWise.Api.Repositories.Interface
{
    public interface IUserLearningStatsRepository
    {
        Task<UserLearningStats> GetByUserIdAsync(string userId);
        Task UpdateAsync(UserLearningStats stats);

        Task<UserLearningStats> CreateAsync(UserLearningStats userLearningStats);

    }
}
