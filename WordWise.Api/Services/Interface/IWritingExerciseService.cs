namespace WordWise.Api.Services.Interface
{
    public interface IWritingExerciseService
    {
        Task<string> GetFeedBackFromAi(Guid writingExerciseId);
    }
}
