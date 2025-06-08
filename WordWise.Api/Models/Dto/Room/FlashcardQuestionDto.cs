namespace WordWise.Api.Models.Dto.Room
{
    public class FlashcardQuestionDto
    {
        public int FlashcardId { get; set; }
        public string QuestionText { get; set; } = string.Empty; // Sẽ là Term hoặc Definition tùy theo RoomMode
        public string? ExampleSentence { get; set; }
    }
}
