using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IMultipleChoiceTestRepository
    {
        Task<MultipleChoiceTest?> CreateAsync(MultipleChoiceTest multipleChoiceTest);

        Task<MultipleChoiceTest?> GetByIdAsync(Guid id);

        Task<bool> DeleteAsync(Guid multipleChoiceTestId, string userId);
    }
}
