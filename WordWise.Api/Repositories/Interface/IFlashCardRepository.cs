using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IFlashCardRepository
    {
        Task<Flashcard?> CreateAsync(Flashcard flashcard);
        Task<Flashcard?> UpdateAsync(Flashcard flashcard, string userId);
        Task<Flashcard?> DeleteAsync(int flashCardId, string userId);
        Task<Flashcard?> GetAsync(int flashCardId);
        Task<IEnumerable<Flashcard>> GetAllByFlashcardSetIdAsync(Guid flashcardSetId);
        Task<bool> DeleteAllFlashCardAync(Guid flashcardSetId);
        Task<int> CreateRangeAsync(Guid flashcardSetId, ICollection<Flashcard> flashcards);

        Task<bool> CheckUserPermissions(string userId, Guid flashcardSetId);
    }
}
