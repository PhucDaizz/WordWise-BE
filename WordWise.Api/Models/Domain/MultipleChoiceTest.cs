using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class MultipleChoiceTest
    {
        [Key]
        public Guid MultipleChoiceTestId { get; set; }
        public string UserId { get; set; }

        [MaxLength(250)]
        public string? Title { get; set; }
        public string? Content { get; set; }
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsPublic { get; set; } = true;
        public int? LearnerCount { get; set; } = 0;
        public LanguageLevel Level { get; set; } 

        // Navigation properties
        public IEnumerable<Question> Questions{ get; set; }
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
    }
}
