using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.Question;

namespace WordWise.Api.Models.Dto.MultipleChoiceTest
{
    public class MultipleChoiceTestDto
    {
        public Guid MultipleChoiceTestId { get; set; }
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsPublic { get; set; } = true;
        public IEnumerable<QuestionDto> Questions { get; set; }
    }
}
