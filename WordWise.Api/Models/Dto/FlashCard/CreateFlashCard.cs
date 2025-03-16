namespace WordWise.Api.Models.Dto.FlashCard
{
    public class CreateFlashCard
    {
        public string Term { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public Guid FlashcardSetId { get; set; }
    }
}
