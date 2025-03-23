using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class WriteContent
    {
        [Required]
        public string Content { get; set; }
    }
}
