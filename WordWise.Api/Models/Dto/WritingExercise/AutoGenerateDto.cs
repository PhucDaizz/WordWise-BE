﻿using System.ComponentModel.DataAnnotations;

namespace WordWise.Api.Models.Dto.WritingExercise
{
    public class AutoGenerateDto
    {
        [Required]
        [MaxLength(50)]
        public string LearningLanguage { get; set; }
        [Required]
        [MaxLength(50)]
        public string NativeLanguage { get; set; }
    }
}
