using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCardSet;

namespace WordWise.Api.Services.Interface
{
    public interface IFlashcardSetService
    {
        Task<FlashcardSet> DeleteByIdAsync(Guid id, string userId, bool isAdmin = false);

        Task<FlashcardSet?> AutoGenerateByAi(FlashcardSet flashcardSet);

        Task<FlashcardSet?> GetByIdAsync(Guid flashcardSetId, string userId, bool isAdmin = false);
    }
}
