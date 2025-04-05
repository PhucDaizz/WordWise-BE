using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashCardSet
{
    public class CreateFlashCardSetDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool IsPublic { get; set; } = true;
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        [Required]
        [Range(1, 6)]
        public int Level { get; set; }
    }
}
