namespace WordWise.Api.Models.Dto.FlashCard
{
    public class FlashCardDto
    {
        public int FlashcardId { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
