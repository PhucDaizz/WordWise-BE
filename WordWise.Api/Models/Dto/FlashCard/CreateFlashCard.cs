using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashCard
{
    public class CreateFlashCard
    {
        [MaxLength(70)]
        public string Term { get; set; }
        [MaxLength(200)]
        public string Definition { get; set; }
        [MaxLength(200)]
        public string Example { get; set; }
        [Required]
        public Guid FlashcardSetId { get; set; }
    }
}
