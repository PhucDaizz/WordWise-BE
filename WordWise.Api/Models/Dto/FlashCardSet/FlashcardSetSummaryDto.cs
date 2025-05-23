﻿namespace WordWise.Api.Models.Dto.FlashCardSet
{
    public class FlashcardSetSummaryDto
    {
        public Guid FlashcardSetId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LearningLanguage { get; set; }
        public string NativeLanguage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TotalVocabulary { get; set; }
        public int? LearnerCount { get; set; }
        public int Level { get; set; }
    }
}
