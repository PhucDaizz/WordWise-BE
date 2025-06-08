namespace WordWise.Api.Models.Dto.Room
{
    public class RoomDetailsDto: RoomDto
    {
        public List<RoomParticipantDto> Participants { get; set; } = new List<RoomParticipantDto>();
        public FlashcardQuestionDto? CurrentFlashcard { get; set; }
    }
}
