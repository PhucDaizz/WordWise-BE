using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Domain
{
    public class StudentFlashcardAttempt
    {
        [Key]
        public Guid AttemptId { get; set; }
        public Guid RoomParticipantId { get; set; }
        public int FlashcardId { get; set; }
        public Guid RoomId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int? TimeTakenMs { get; set; }

        // Navigation properties
        public RoomParticipant RoomParticipant { get; set; }
        public Flashcard Flashcard { get; set; }
        public Room Room { get; set; }
    }
}
