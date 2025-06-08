using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.Room
{
    public class RoomDto
    {
        public Guid RoomId { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string? RoomName { get; set; }
        public Guid FlashcardSetId { get; set; }
        public string? FlashcardSetName { get; set; } 
        public string TeacherId { get; set; } = string.Empty;
        public string? TeacherName { get; set; } 
        public RoomStatus Status { get; set; }
        public RoomMode Mode { get; set; }
        public int? MaxParticipants { get; set; }
        public int CurrentParticipantCount { get; set; }
        public bool ShowLeaderboardRealtime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CurrentQuestionIndex { get; set; }
    }
}
