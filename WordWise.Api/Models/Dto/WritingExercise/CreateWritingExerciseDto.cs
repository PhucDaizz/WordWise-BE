using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class CreateWritingExerciseDto
    {
        [Required]
        [StringLength(230)]
        public string Topic { get; set; }
        public string? Content { get; set; }
        public string? AIFeedback { get; set; }

        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
    }
}
