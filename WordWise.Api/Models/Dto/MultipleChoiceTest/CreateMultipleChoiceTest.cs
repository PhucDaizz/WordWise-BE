using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.MultipleChoiceTest
{
    public class CreateMultipleChoiceTest
    {
        [MaxLength(250)]
        public string? Title { get; set; }
        public string? Content { get; set; }
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        public bool IsPublic { get; set; } = true;
        [Required]
        [Range(1, 6)]
        public int Level { get; set; }
    }
}
