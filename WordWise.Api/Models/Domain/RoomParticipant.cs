using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class RoomParticipant
    {
        public Guid RoomParticipantId { get; set; }
        public Guid RoomId { get; set; }
        public string UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Score { get; set; } = 0;
        public RoomParticipantStatus Status { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public int CurrentQuestionIndex { get; set; } = -1;
        public DateTime? LastAnswerSubmittedAt { get; set; }
        // Navigation properties
        public Room Room { get; set; }
        public ExtendedIdentityUser User { get; set; }
        public ICollection<StudentFlashcardAttempt> StudentFlashcardAttempts { get; set; } = new List<StudentFlashcardAttempt>();

    }
}
