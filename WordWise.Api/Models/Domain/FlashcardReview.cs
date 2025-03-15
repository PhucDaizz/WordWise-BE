using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordWise.Api.Models.Domain
{
    public class FlashcardReview
    {
        [Key]
        public Guid FlashcardReviewId { get; set; }
        public string UserId { get; set; }
        public Guid FlashcardSetId { get; set; }

        [Range(1,5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }
        public DateTime CreateAt { get; set; }

        // Navigation properties
        [ForeignKey("FlashcardSetId")]
        public FlashcardSet FlashcardSet { get; set; }
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
    }
}
