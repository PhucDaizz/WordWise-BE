using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordWise.Api.Models.Domain
{
    public class MultipleChoiceTest
    {
        [Key]
        public Guid MultipleChoiceTestId { get; set; }
        public string UserId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Content { get; set; }
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsPublic { get; set; }

        // Navigation properties
        public IEnumerable<Question> Questions{ get; set; }
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
    }
}
