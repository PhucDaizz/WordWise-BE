using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCardSet;

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
        Task<GetAllFlashCardSetDto> GetPublicAsync(string? learningLanguage, string? nativeLanguage, int currentPage = 1, int itemPerPage = 20);
        Task<GetAllFlashCardSetDto?> GetSummaryAsync(string userIdFind, string? yourUserId, int currentPage = 1, int itemPerPage = 5);
    }
}
