using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class FlashcardSet
    {
        [Key]
        public Guid FlashcardSetId { get; set; }
        public string UserId { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        public int? LearnerCount { get; set; } = 0;
        public LanguageLevel? Level { get; set; }
        // Navigation properties
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
        public ICollection<Flashcard> Flashcards { get; set; }
        public ICollection<FlashcardReview> FlashcardReviews { get; set; }
        public ICollection<Room> Rooms { get; set; }

    }
}
