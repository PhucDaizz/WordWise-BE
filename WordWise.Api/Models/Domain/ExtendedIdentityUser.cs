using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Enum;


namespace WordWise.Api.Models.Domain
{
    public class ExtendedIdentityUser: IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        [Range(1,6)]
        public LanguageLevel Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Gender { get; set; }

        // Navigation properties

        public ICollection<FlashcardReview> FlashcardReviews { get; set; }
        public ICollection<FlashcardSet> FlashcardSets { get; set; }
        public ICollection<WritingExercise> WritingExercises { get; set; }
        public ICollection<MultipleChoiceTest> MultipleChoiceTests { get; set; }
        public ICollection<ContentReport> ContentReports { get; set; }
        public UserLearningStats UserLearningStats { get; set; }
        public ICollection<RoomParticipant> RoomParticipants { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
