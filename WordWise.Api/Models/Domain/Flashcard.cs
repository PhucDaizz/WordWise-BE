using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordWise.Api.Models.Domain
{
    public class Flashcard
    {
        [Key]
        public int FlashcardId { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid FlashcardSetId { get; set; }

        // Navigation property
        [ForeignKey("FlashcardSetId")]
        public FlashcardSet FlashcardSet { get; set; }
        public ICollection<StudentFlashcardAttempt> StudentFlashcardAttempts { get; set; }

    }
}
