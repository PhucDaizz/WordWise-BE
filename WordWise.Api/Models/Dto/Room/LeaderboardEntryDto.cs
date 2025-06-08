namespace WordWise.Api.Models.Dto.Room
{
    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public int Score { get; set; }
    }
}
