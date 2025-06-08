using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IStudentFlashcardAttemptRepository
    {
        Task AddAsync(StudentFlashcardAttempt attempt);
        void RemoveRange(IEnumerable<StudentFlashcardAttempt> attempts);
        Task<IEnumerable<StudentFlashcardAttempt>> GetAttemptsByParticipantIdsAsync(IEnumerable<Guid> participantIds);
    }
}
