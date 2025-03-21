using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashCardSet
{
    public class UpdateFlashcardSet
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
    }
}
