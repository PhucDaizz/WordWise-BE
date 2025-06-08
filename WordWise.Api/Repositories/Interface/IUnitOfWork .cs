using WordWise.Api.Repositories.Implement;

namespace WordWise.Api.Repositories.Interface
{
    public interface IUnitOfWork: IDisposable
    {
        IRoomRepository Rooms { get; }
        IRoomParticipantRepository RoomParticipants { get; }
        IStudentFlashcardAttemptRepository StudentFlashcardAttempts { get; }
        IFlashcardSetRepository FlashcardSets { get; }
        IFlashCardRepository Flashcards { get; }
        IAuthRepository Auth { get; }

        Task<int> CompleteAsync();
    }
}
