using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IFlashCardRepository
    {
        Task<Flashcard> CreateAsync(Flashcard flashcard);
        Task<Flashcard?> UpdateAsync(Flashcard flashcard);
        Task<Flashcard?> DeleteAsync(int flashCardId);
        Task<Flashcard?> GetAsync(int flashCardId);
        Task<IEnumerable<Flashcard>> GetAllByFlashcardSetIdAsync(Guid flashcardSetId);
    }
}
