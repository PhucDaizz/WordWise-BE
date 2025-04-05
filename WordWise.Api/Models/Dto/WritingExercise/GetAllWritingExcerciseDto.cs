namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class GetAllWritingExcerciseDto
    {
        public IEnumerable<WritingExcerciseSummaryDto> writingExcercises { get; set; }
        public int CurentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalPage { get; set; }
    }
}
