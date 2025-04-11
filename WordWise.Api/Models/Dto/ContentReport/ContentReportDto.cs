using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.ContentReport
{
    public class ContentReportDto
    {
        public Guid ContentReportId { get; set; }
        public string UserId { get; set; }
        public string ContentId { get; set; }
        public string ContentType { get; set; }
        public string Reason { get; set; }
        public ReportStatus Status { get; set; } 
        public DateTime CreateAt { get; set; }
    }
}
