using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IFlashcardSetRepository
    {
        Task<FlashcardSet?> CreateAsync(FlashcardSet flashcardSet);
        Task<FlashcardSet?> UpdateAsync(FlashcardSet flashcardSet);
        Task<FlashcardSet?> DeleteAsync(Guid flashcardSetId, string userId);
        Task<FlashcardSet?> GetAsync(Guid flashcardSetId);
        Task<IEnumerable<FlashcardSet>> GetAllByUserIdAsync(string userId);
        Task<bool> ChangeStatusAsync(Guid flashcardSetId, string userId);
    }
}
