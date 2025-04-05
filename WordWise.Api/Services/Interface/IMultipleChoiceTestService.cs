using WordWise.Api.Models.Domain;

namespace WordWise.Api.Services.Interface
{
    public interface IMultipleChoiceTestService
    {
        Task<MultipleChoiceTest?> GenerateByAIAsync(string userId, string LearningLanguage, string NativeLanguage, int level, string? title);
    }
}
