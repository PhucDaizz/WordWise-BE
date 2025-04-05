namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class WritingExcerciseSummaryDto
    {
        public Guid WritingExerciseId { get; set; }
        public string Topic { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; }
    }
}
