namespace WordWise.Api.Models.Dto.UserLearningstats
{
    public class SessionResultDto
    {
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeFinish { get; set; }
        public double? DurationMinutes { get; set; }
        public double TotalLearningMinutes { get; set; }
        public int CurrentStreak { get; set; }
    }
}
