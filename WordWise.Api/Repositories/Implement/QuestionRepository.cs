using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMultipleChoiceTestRepository _multipleChoiceTestRepository;
        private readonly WordWiseDbContext dbContext;

        public QuestionRepository(IMultipleChoiceTestRepository multipleChoiceTestRepository, WordWiseDbContext dbContext)
        {
            _multipleChoiceTestRepository = multipleChoiceTestRepository;
            this.dbContext = dbContext;
        }
        public async Task<Question?> CreateAsync(Question question, string userId)
        {
            var multipleChoiceTest = await _multipleChoiceTestRepository.GetByIdAsync(question.MultipleChoiceTestId);
            if (multipleChoiceTest == null || multipleChoiceTest.UserId != userId)
            {
                return null;
            }
            else
            {
                if(multipleChoiceTest.Questions.Count() >= 5)
                {
                    return null;
                }
                await dbContext.Questions.AddAsync(question);
                await dbContext.SaveChangesAsync();
                return question;
            }
        }

        public async Task<IEnumerable<Question>?> CreateRangeAsync(IList<Question> questions, string userId, Guid multipleChoiceTestId)
        {
            var multipleChoiceTest = await _multipleChoiceTestRepository.GetByIdAsync(multipleChoiceTestId);
            if (multipleChoiceTest == null || multipleChoiceTest.UserId != userId)
            {
                return null;
            }
            else
            {
                foreach (var question in questions)
                {
                    question.MultipleChoiceTestId = multipleChoiceTestId;
                }

                var countQuestions = multipleChoiceTest.Questions.Count();
                if (countQuestions >= 5)
                {
                    return null;
                }
                int takeItem = 5 - countQuestions;
                questions = questions.Take(takeItem).ToList();
                await dbContext.Questions.AddRangeAsync(questions);
                await dbContext.SaveChangesAsync();

                return questions;
            }
        }

        public async Task<bool> DeleteAsync(Guid questionId, string userId)
        {
            var question = await dbContext.Questions
                .Where(q => q.QuestionId == questionId)
                .Select(q => new { q, q.MultipleChoiceTest.UserId }) // Chỉ lấy UserId
                .FirstOrDefaultAsync();

            if (question == null || question.UserId != userId)
            {
                return false;
            }

            dbContext.Questions.Remove(question.q);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Question>?> GetAllAsync(Guid multipleChoiceTestId)
        {
            var questions = await dbContext.Questions
                .Where(q => q.MultipleChoiceTestId == multipleChoiceTestId)
                .ToListAsync();
            if (questions == null || questions.Count == 0)
            {
                return null;
            }
            return questions;
        }

        public async Task<Question?> UpdateAsync(Question question, string userId)
        {
            var existing = await dbContext.Questions
                .Where(q => q.QuestionId == question.QuestionId)
                .Select(q => new {q, q.MultipleChoiceTest.UserId })
                .FirstOrDefaultAsync();

            if (existing == null || existing.UserId != userId)
            {
                return null; 
            }

            question.MultipleChoiceTestId = existing.q.MultipleChoiceTestId;

            dbContext.Entry(existing.q).CurrentValues.SetValues(question);
            await dbContext.SaveChangesAsync();

            return question;
        }
    }
}
