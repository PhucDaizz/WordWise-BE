using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordWise.Api.Models.Domain
{
    public class UserLearningStats
    {
        [Key]
        public string UserId { get; set; }
        [Range(0, int.MaxValue)]
        public int CurrentStreak { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int LongestStreak { get; set; } = 0;
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(10, 2)")]
        public double TotalLearningMinutes { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastLearningDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? SessionStartTime { get; set; } 
        [DataType(DataType.DateTime)]
        public DateTime? SessionEndTime { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
    }
}
