using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.WritingExercise;

namespace WordWise.Api.Repositories.Interface
{
    public interface IWritingExerciseRepository
    {
        Task<WritingExercise?> CreateAsync(WritingExercise writingExercise);
        Task<bool> DeleteAsync(Guid writingExerciseId, string userId);
        Task<bool> UpdateFeedback(Guid writingExerciseId, string feedback);
        Task<bool> UpdateContent(Guid writingExerciseId, string content);
        Task<bool> UpdateTopic(Guid writingExerciseId, string topic);
        Task<WritingExercise?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Guid writingExerciseId, string userId , UpdateWritingExercise updateWritingExercise);

    }
}
