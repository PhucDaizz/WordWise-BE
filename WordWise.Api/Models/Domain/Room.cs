using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class Room
    {
        public Guid RoomId { get; set; }
        public string UserId { get; set; }
        public Guid FlashcardSetId { get; set; }
        public string RoomCode { get; set; }
        public string? RoomName { get; set; }
        public RoomStatus Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public RoomMode Mode { get; set; }
        public int? MaxParticipants { get; set; }
        public int CurrentQuestionIndex { get; set; } = 0;
        public bool ShowLeaderboardRealtime { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public FlashcardSet FlashcardSet { get; set; }
        public ICollection<RoomParticipant> RoomParticipants { get; set; } = new List<RoomParticipant>();
        public ExtendedIdentityUser User { get; set; }
        public ICollection<StudentFlashcardAttempt> StudentFlashcardAttempts { get; set; } = new List<StudentFlashcardAttempt>();

    }
}
