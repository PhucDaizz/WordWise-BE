using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class WritingExerciseDto
    {
        public Guid WritingExerciseId { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string? Content { get; set; }
        public string? AIFeedback { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
