using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.Room
{
    public class RoomParticipantDto
    {
        public Guid RoomParticipantId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Username { get; set; }
        public int Score { get; set; }
        public RoomParticipantStatus Status { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }
}
