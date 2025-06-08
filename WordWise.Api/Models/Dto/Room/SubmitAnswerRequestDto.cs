namespace WordWise.Api.Models.Dto.Room
{
    public class SubmitAnswerRequestDto
    {
        public int FlashcardId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
    }
}
