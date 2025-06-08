using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.Room
{
    public class CreateRoomRequestDto
    {
        public Guid FlashcardSetId { get; set; }
        public string? RoomName { get; set; } 
        public RoomMode Mode { get; set; } = RoomMode.TermToDefinition;
        public int? MaxParticipants { get; set; } 
        public bool ShowLeaderboardRealtime { get; set; } = true;
    }
}
