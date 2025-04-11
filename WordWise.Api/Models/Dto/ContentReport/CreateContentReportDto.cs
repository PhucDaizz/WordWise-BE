using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Dto.ContentReport
{
    public class CreateContentReportDto
    {
        [Required]
        [Range(1,2)]
        public ContentTypeReport ContentType { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }
    }
}
