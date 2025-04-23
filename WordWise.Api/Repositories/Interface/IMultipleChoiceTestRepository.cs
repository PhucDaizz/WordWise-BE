using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.MultipleChoiceTest;

namespace WordWise.Api.Repositories.Interface
{
    public interface IMultipleChoiceTestRepository
    {
        Task<MultipleChoiceTest?> CreateAsync(MultipleChoiceTest multipleChoiceTest, bool isAI = false);

        Task<MultipleChoiceTest?> GetByIdAsync(Guid id);

        Task<bool> DeleteAsync(Guid multipleChoiceTestId, string userId);

        Task<GetAllMultipleChoiceTestDto?> GetSummaryAsync(string userIdFind, string? yourUserId, int currentPage = 1, int itemPerPage = 5);
        Task<bool> SetStatusAsync(Guid multipleChoiceTestId, string userId);
        Task<GetAllMultipleChoiceTestDto> GetPublicAsync(string? learningLanguage, string? nativeLanguage, int currentPage = 1, int itemPerPage = 20);
        Task<GetAllMultipleChoiceTestDto> GetAllAdminAsync(Guid? multipleChoiceTestId, string? learningLanguage, string? nativeLanguage, int page = 1, int itemPerPage = 20);

    }
}
