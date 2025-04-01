using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.FlashCardSet
{
    public class GetAllFlashCardSetDto
    {
        public IEnumerable<FlashcardSetSummaryDto> FlashcardSets { get; set; }
        public string? UserName { get; set; }
        public int CurentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalPage { get; set; }
    }
}
