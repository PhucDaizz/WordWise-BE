namespace WordWise.Api.Models.Dto.Room
{
    public class AnswerProcessingResultDto
    {
        public bool IsCorrect { get; set; }
        public string? CorrectAnswer { get; set; } // Gửi đáp án đúng (ví dụ: Definition hoặc Term tùy theo mode)
        public int NewScore { get; set; }
        public int NextFlashcardId { get; set; } // ID của flashcard tiếp theo, hoặc Guid.Empty nếu hết
        public FlashcardQuestionDto? NextQuestion { get; set; }
        public bool HasFinished { get; set; } = false;
    }
}
