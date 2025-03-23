using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class UpdateWritingExercise
    {
        public string Topic { get; set; }
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
    }
}
