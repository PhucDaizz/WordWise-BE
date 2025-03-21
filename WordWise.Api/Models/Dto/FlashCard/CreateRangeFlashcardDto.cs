using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashCard
{
    public class CreateRangeFlashcardDto
    {
        [MaxLength(70)]
        public string Term { get; set; }
        [MaxLength(200)]
        public string Definition { get; set; }
        [MaxLength(200)]
        public string Example { get; set; }
    }
}
