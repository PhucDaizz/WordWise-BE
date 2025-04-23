using System.Collections;
using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IQuestionRepository
    {
        Task<Question?> CreateAsync(Question question, string userId);
        Task<Question?> UpdateAsync(Question question, string userId);
        Task<bool> DeleteAsync(Guid questionId, string userId);
        Task<IEnumerable<Question>?> CreateRangeAsync(IList<Question> questions, string userId, Guid multipleChoiceTestId);
        Task<IEnumerable<Question>?> GetAllAsync(Guid multipleChoiceTestId);

    }
}
