using System.ComponentModel.DataAnnotations.Schema;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class ContentReport
    {
        public Guid ContentReportId { get; set; }
        public string UserId { get; set; }
        public string ContentId { get; set; }
        public ContentTypeReport ContentType { get; set; }
        public string Reason { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public DateTime CreateAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public ExtendedIdentityUser User { get; set; }
    }
}
