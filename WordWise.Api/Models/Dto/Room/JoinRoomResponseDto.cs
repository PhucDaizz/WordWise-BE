namespace WordWise.Api.Models.Dto.Room
{
    public class JoinRoomResponseDto
    {
        public RoomDto RoomInfo { get; set; } = null!;
        public RoomParticipantDto ParticipantInfo { get; set; } = null!;
        public FlashcardQuestionDto? CurrentQuestion { get; set; }
    }
}
