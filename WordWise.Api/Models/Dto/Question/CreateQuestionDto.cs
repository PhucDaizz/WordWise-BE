using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.Question
{
    public class CreateQuestionDto
    {
        [Required]
        [MaxLength(2000)]
        public string QuestionText { get; set; }

        [MaxLength(500)]
        public string? Answer_a { get; set; }
        [MaxLength(500)]
        public string? Answer_b { get; set; }
        [MaxLength(500)]
        public string? Answer_c { get; set; }
        [MaxLength(500)]
        public string? Answer_d { get; set; }
        [Range(1,4)]
        public AnswerKey? CorrectAnswer { get; set; }
        [MaxLength(1000)]
        public string? Explanation { get; set; }
    }
}
