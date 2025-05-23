﻿using System.ComponentModel.DataAnnotations;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Models.Domain
{
    public class Question
    {
        [Key]
        public Guid QuestionId { get; set; }
        public Guid MultipleChoiceTestId { get; set; }
        public string QuestionText { get; set; }

        [MaxLength(500)]
        public string? Answer_a { get; set; }
        [MaxLength(500)]
        public string? Answer_b { get; set; }
        [MaxLength(500)]
        public string? Answer_c { get; set; }
        [MaxLength(500)]
        public string? Answer_d { get; set; }
        public AnswerKey? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }

        // Navigation property
        public MultipleChoiceTest MultipleChoiceTest{ get; set; }
    }
}
