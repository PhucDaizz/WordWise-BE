using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.WritingExercise;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class WritingExerciseRepository : IWritingExerciseRepository
    {
        private readonly WordWiseDbContext dbContext;

        public WritingExerciseRepository(WordWiseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<WritingExercise?> CreateAsync(WritingExercise writingExercise)
        {
            var numberOfWriteEx = await dbContext.WritingExercises.Where(x => x.UserId == writingExercise.UserId).CountAsync();
            if (numberOfWriteEx >= 5)
            {
                return null;
            }

            var writingEx = await dbContext.WritingExercises.AddAsync(writingExercise);
            await dbContext.SaveChangesAsync();
            return writingEx.Entity;
        }

        public async Task<bool> DeleteAsync(Guid writingExerciseId, string userId)
        {
            var existing = await dbContext.WritingExercises
                .FirstOrDefaultAsync(x => x.WritingExerciseId == writingExerciseId && x.UserId == userId);
            if (existing == null)
            {
                return false;
            }
            dbContext.WritingExercises.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<WritingExercise?> GetByIdAsync(Guid id)
        {
            var existing =await dbContext.WritingExercises.FirstOrDefaultAsync(x => x.WritingExerciseId == id);
            if(existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<bool> UpdateAsync(Guid writingExerciseId, string userId, UpdateWritingExercise updateWritingExercise)
        {
            var existing = await dbContext.WritingExercises.FirstOrDefaultAsync(x => x.WritingExerciseId == writingExerciseId && x.UserId == userId);
            if(existing == null)
            {
                return false;
            }
            existing.Topic = updateWritingExercise.Topic;
            existing.LearningLanguage = updateWritingExercise.LearningLanguage;
            existing.NativeLanguage = updateWritingExercise.NativeLanguage;
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateContent(Guid writingExerciseId, string content)
        {
            // Validate content
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Feedback cannot be null or empty.", nameof(content));
            }
            try
            {
                var existing = await dbContext.WritingExercises.FirstOrDefaultAsync(x => x.WritingExerciseId == writingExerciseId);
                if (existing == null)
                {
                    return false;
                }

                existing.Content = content;
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateFeedback(Guid writingExerciseId, string feedback)
        {
            // Validate feedback
            if (string.IsNullOrWhiteSpace(feedback))
            {
                throw new ArgumentException("Feedback cannot be null or empty.", nameof(feedback));
            }

            try
            {
                var existing = await dbContext.WritingExercises.FirstOrDefaultAsync(x => x.WritingExerciseId == writingExerciseId);
                if (existing == null)
                {
                    return false;
                }
                
                existing.AIFeedback = feedback;
                await dbContext.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
