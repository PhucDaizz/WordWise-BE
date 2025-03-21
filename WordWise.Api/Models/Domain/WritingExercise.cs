using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordWise.Api.Models.Domain
{
    public class WritingExercise
    {
        [Key]
        public Guid WritingExerciseId { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string? Content { get; set; }
        public int? AIFeedback { get; set; }

        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User{ get; set; }
    }
}
