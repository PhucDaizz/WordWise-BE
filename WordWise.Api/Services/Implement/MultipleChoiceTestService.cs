using Azure;
using Gemini.NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Models.Enums;
using System.Net;
using System.Text.RegularExpressions;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.Question;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class MultipleChoiceTestService : IMultipleChoiceTestService
    {
        private readonly IMultipleChoiceTestRepository _multipleChoiceTestRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICacheService _cacheService;
        private readonly WordWiseDbContext dbContext;

        public MultipleChoiceTestService(IMultipleChoiceTestRepository multipleChoiceTestRepository, IQuestionRepository questionRepository, ICacheService cacheService, WordWiseDbContext dbContext)
        {
            _multipleChoiceTestRepository = multipleChoiceTestRepository;
            _questionRepository = questionRepository;
            _cacheService = cacheService;
            this.dbContext = dbContext;
        }

        public async Task<MultipleChoiceTest?> GenerateByAIAsync(string userId, string LearningLanguage, string NativeLanguage, string? title)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId is missing for the writing exercise.");
            }

            if (string.IsNullOrEmpty(LearningLanguage))
            {
                throw new ArgumentException("LearningLanguage is missing for the writing exercise.");
            }

            if (string.IsNullOrEmpty(NativeLanguage)) {
                throw new ArgumentException("NativeLanguage is missing for the writing exercise.");
            }

            // Check Limit create test for user
            var tests = await dbContext.MultipleChoiceTests.Where(x => x.UserId == userId).CountAsync();
            if(tests >= 5)
            {
                return null;
            }

            // Get API Key
            string apiKey;
            bool hasApiKey = _cacheService.TryGetApiKey(userId, out apiKey);
            if (!hasApiKey)
            {
                throw new InvalidOperationException("API Key not found for this user.");
            }

            // Validate API Key
            var generator = new Generator(apiKey);
            bool isValid = await generator.IsValidApiKeyAsync();
            if (!isValid)
            {
                throw new InvalidOperationException("API key is invalid or expired.");
            }

            string sanitizedTitle = WebUtility.HtmlEncode(title ?? "Ngẫu nhiên");

            string prompt =
                $@"
                    Bạn là một chuyên gia ngôn ngữ hàng đầu, chuyên tạo đề thi quốc tế.  
                    Hãy giúp tôi tạo một bài đọc về ""{sanitizedTitle ?? "Ngẫu nhiên"}"".  
                    Bài đọc phải dài từ 250-350 từ, đúng chuẩn bài thi quốc tế, và phù hợp cho người có trình độ cấp 5/6.  
                    Ngôn ngữ bài đọc: {LearningLanguage}.  
                    Hãy đảm bảo nội dung bài đọc có tính học thuật và liên quan đến {sanitizedTitle}.  
                    LƯU Ý QUAN TRỌNG:
                    - Tiêu đề phải GIỮ NGUYÊN như đã cung cấp, không thêm bất kỳ từ ngữ nào khác
                    - Không thêm câu giới thiệu như 'Tuyệt vời!', 'Chúng ta hãy bắt đầu...'

                    Sau bài đọc, hãy tạo **5 câu hỏi trắc nghiệm**. Mỗi câu hỏi có 4 lựa chọn **A, B, C, D**, và chỉ có 1 đáp án đúng.  
                    - Đáp án đúng phải được phân bố ngẫu nhiên (ít nhất 1 câu trả lời A, B, C, D trong 5 câu). 
                    Lời giải thích phải được viết bằng {NativeLanguage} và **phải chỉ rõ dấu hiệu nhận biết trong bài đọc**, nhưng không có dòng riêng biệt.  
                    Dấu hiệu nhận biết phải được gộp vào phần giải thích, làm cho nó mạch lạc và tự nhiên như một lời hướng dẫn.  
                    Sử dụng định dạng sau để dễ dàng tách nội dung:  

                    ###START###  
                    Đề bài đọc 

                    ###QUESTIONS###  
                    1. [Câu hỏi 1]  
                       A. [Đáp án]  
                       B. [Đáp án]  
                       C. [Đáp án]  
                       D. [Đáp án]  
                       **Correct Answer: X**  
                       **Giải thích:** [Lời giải thích bằng {NativeLanguage}, trong đó có dấu hiệu nhận biết giúp thí sinh tìm ra đáp án trong bài đọc].  

                    2. [Câu hỏi 2]  
                       ...  

                    Hãy đi thẳng vào yêu cầu và tạo bài thi đúng chuẩn quốc tế.

                ";

            var apiRequestBuilder = new ApiRequestBuilder()
                .WithPrompt(prompt)
                .WithDefaultGenerationConfig(temperature: 0.85F)
                .Build();

            var modelVersion = ModelVersion.Gemini_20_Flash;

            await using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var response = await generator.GenerateContentAsync(apiRequestBuilder, modelVersion);

                if (string.IsNullOrEmpty(response?.Result))
                {
                    throw new InvalidOperationException("AI response is empty or invalid.");
                }

                var result = ParseQuiz(response.Result);



                // Create a new multiple choice test
                var multipleChoiceTest = await _multipleChoiceTestRepository.CreateAsync(new MultipleChoiceTest
                {
                    UserId = userId,
                    Title = result.Title,
                    Content = result.ReadingText,
                    LearningLanguage = LearningLanguage,
                    NativeLanguage = NativeLanguage,
                    CreateAt = DateTime.UtcNow,
                    IsPublic = true,
                }, true);

                // Create questions
                await _questionRepository.CreateRangeAsync(result.Questions, userId, multipleChoiceTest.MultipleChoiceTestId);

                await trans.CommitAsync();

                return multipleChoiceTest;

            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw new InvalidOperationException("An error occurred while generating reading test.");
            }


        }


        private (string Title, string ReadingText, List<Question> Questions) ParseQuiz(string input)
        {
            var sections = input.Split("###QUESTIONS###", StringSplitOptions.RemoveEmptyEntries);
            if (sections.Length < 2)
            {
                throw new Exception("Dữ liệu không đúng định dạng!");
            }

            // Get reading text
            string readingText = sections[0].Replace("###START###", "").Trim();

            // Get title of the reading (first line remove **Passage: ** and ** at the end)
            var lines = readingText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            string title = lines[0].Replace("**Passage: ", "").Replace("**", "").Trim();
            string content = string.Join("\n", lines.Skip(1)).Trim();

            // Handle question part
            string questionsText = sections[1].Trim();

            var questionPattern = new Regex(
                @"(\d+)\.\s*(.*?)\n\s*A\.\s*(.*?)\n\s*B\.\s*(.*?)\n\s*C\.\s*(.*?)\n\s*D\.\s*(.*?)\n\s*\*\*Correct Answer:\s*(\w+)\*\*\s*\*\*Giải thích:\*\*\s*(.*?)(?=\n\d+\.|\n*$)",
                RegexOptions.Singleline
            );

            var matches = questionPattern.Matches(questionsText);

            List<Question> questions = new List<Question>();
            foreach (Match match in matches)
            {
                questions.Add(new Question
                {
                    QuestionText = match.Groups[2].Value.Trim(),
                    Answer_a = match.Groups[3].Value.Trim(),
                    Answer_b = match.Groups[4].Value.Trim(),
                    Answer_c = match.Groups[5].Value.Trim(),
                    Answer_d = match.Groups[6].Value.Trim(),
                    CorrectAnswer = Enum.TryParse<AnswerKey>(match.Groups[7].Value.Trim(), out var key) ? key : AnswerKey.A,
                    Explanation = match.Groups[8].Value.Trim()
                });
            }

            return (title, content, questions);
        }

    }
}
