using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.AI
{
    public class GenerateFlashcardSetRequest
    {
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Title { get; set; }
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }

    }
}
