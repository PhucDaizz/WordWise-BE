namespace WordWise.Api.Models.Dto.UserLearningstats
{
    public class Statistics
    {
        public int TotalUser { get; set; }
        public int TotalFlashcardSet { get; set; }
        public int TotalMultichoiceTest { get; set; }
        public int TotalWritingTest { get; set; }
        public int TotalReport { get; set; }
        public double AverageTotalLearningMinutes { get; set; } 
        public double AverageCurrentStreak { get; set; }




    }
}
