using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.AI
{
    public class GenerateMultipleChoiceTestRequest
    {
        [Required]
        [MaxLength(30)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(30)]
        public string NativeLanguage { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [Required]
        [Range(1, 6, ErrorMessage = "Level must be between 1 and 6.")]
        public int Level { get; set; }

    }
}
