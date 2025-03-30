namespace WordWise.Api.Models.Dto.MultipleChoiceTest
{
    public class MultipleChoiceTestSummaryDto
    {
        public Guid MultipleChoiceTestId { get; set; }
        public string Title { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsPublic { get; set; }
    }
}
