using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashcardReview
{
    public class CreateFlashcardReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(150)]
        public string Comment { get; set; }
    }
}
