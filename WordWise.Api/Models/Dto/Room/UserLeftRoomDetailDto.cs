namespace WordWise.Api.Models.Dto.Room
{
    public class UserLeftRoomDetailDto
    {
        public Guid RoomId { get; set; }
        public RoomParticipantDto LeftParticipant { get; set; } = null!;
    }
}
