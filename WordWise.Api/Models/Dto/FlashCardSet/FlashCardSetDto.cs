using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCard;
using WordWise.Api.Models.Dto.User;
using WordWise.Api.Models.Dto.FlashcardReview;

namespace WordWise.Api.Models.Dto.FlashCardSet
{
    public class FlashCardSetDto
    {
        public Guid FlashcardSetId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public UserDto User { get; set; }
        public int Level { get; set; }

        public ICollection<FlashCardDto> Flashcards { get; set; }
        public ICollection<FlashcardReviewDto> flashcardReviews { get; set; }

    }
}
