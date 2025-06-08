using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class StudentFlashcardAttemptRepository:  IStudentFlashcardAttemptRepository
    {
        private readonly WordWiseDbContext _dbContext;

        public StudentFlashcardAttemptRepository(WordWiseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(StudentFlashcardAttempt attempt)
        {
            await _dbContext.StudentFlashcardAttempts.AddAsync(attempt);
        }

        public async Task<IEnumerable<StudentFlashcardAttempt>> GetAttemptsByParticipantIdsAsync(IEnumerable<Guid> participantIds)
        {
            return await _dbContext.StudentFlashcardAttempts
                                 .Where(sfa => participantIds.Contains(sfa.RoomParticipantId))
                                 .ToListAsync();
        }

        public void RemoveRange(IEnumerable<StudentFlashcardAttempt> attempts)
        {
            _dbContext.StudentFlashcardAttempts.RemoveRange(attempts);
        }
    }
}
