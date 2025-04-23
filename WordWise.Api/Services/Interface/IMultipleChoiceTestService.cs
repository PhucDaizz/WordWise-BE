using WordWise.Api.Models.Domain;

namespace WordWise.Api.Services.Interface
{
    public interface IMultipleChoiceTestService
    {
        Task<MultipleChoiceTest?> GenerateByAIAsync(string userId, string LearningLanguage, string NativeLanguage, int level, string? title);

        Task<MultipleChoiceTest?> GetByIdAsync(Guid multipleChoiceTestId, string? userId, bool isAdmin = false);

        Task<bool> DeleteByIdAsync(Guid multipleChoiceTestId, string userId, bool isAdmin = false);

    }
}
