using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashcardReview
{
    public class FlashcardReviewDto
    {
        public Guid FlashcardReviewId { get; set; }
        public string UserId { get; set; }
        public Guid FlashcardSetId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
