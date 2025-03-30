namespace WordWise.Api.Models.Dto.MultipleChoiceTest
{
    public class GetAllMultipleChoiceTestDto
    {
        public IEnumerable<MultipleChoiceTestSummaryDto> multipleChoiceTestSummaries { get; set; }
        public string? UserName { get; set; }
        public int CurentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalPage { get; set; }
    }
}
