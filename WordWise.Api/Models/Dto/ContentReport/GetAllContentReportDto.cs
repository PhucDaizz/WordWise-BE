namespace WordWise.Api.Models.Dto.ContentReport
{
    public class GetAllContentReportDto
    {
        public IEnumerable<ContentReportDto> ContentReports { get; set; }
        public int CurentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalPage { get; set; }
    }
}
