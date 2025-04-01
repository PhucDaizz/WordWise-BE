using Gemini.NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Enums;
using System.Text.RegularExpressions;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCardSet;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class FlashcardSetService : IFlashcardSetService
    {
        private readonly WordWiseDbContext dbContext;
        private readonly IFlashcardSetRepository _flashcardSetRepository;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly IFlashcardReviewRepository _flashcardReviewRepository;
        private readonly ICacheService _cacheService;

        public FlashcardSetService(WordWiseDbContext dbContext, IFlashcardSetRepository flashcardSetRepository, IFlashCardRepository flashCardRepository, IFlashcardReviewRepository flashcardReviewRepository, ICacheService cacheService)
        {
            this.dbContext = dbContext;
            _flashcardSetRepository = flashcardSetRepository;
            _flashCardRepository = flashCardRepository;
            _flashcardReviewRepository = flashcardReviewRepository;
            _cacheService = cacheService;
        }

        public async Task<FlashcardSet?> AutoGenerateByAi(FlashcardSet flashcardSet)
        {
            if (string.IsNullOrEmpty(flashcardSet.UserId))
            {
                throw new ArgumentException("UserId is missing.");
            }

            if (string.IsNullOrEmpty(flashcardSet.LearningLanguage))
            {
                throw new ArgumentException("LearningLanguage is missing.");
            }

            if (string.IsNullOrEmpty(flashcardSet.NativeLanguage))
            {
                throw new ArgumentException("NativeLanguage is missing.");
            }

            // Get API Key
            string apiKey;
            bool hasApiKey = _cacheService.TryGetApiKey(flashcardSet.UserId, out apiKey);
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

            // Get User Level
            var userLevel = await dbContext.ExtendedIdentityUsers.FirstOrDefaultAsync(x => x.Id == flashcardSet.UserId);
            if (userLevel == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Create flashcard set
            await using var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create Flash card set
                var flcardSet = await _flashcardSetRepository.CreateAsync(flashcardSet);
                if (flcardSet == null)
                {
                    throw new Exception("You have reached the maximum limit of 5 flashcard sets.");
                }

                string prompt = $@"
                    Bạn là một chuyên gia ngôn ngữ hàng đầu thế giới. Hãy tạo cho tôi một bộ flashcard với các thông tin sau:
                        - Ngôn ngữ mẹ đẻ: {flashcardSet.NativeLanguage}
                        - Ngôn ngữ học: {flashcardSet.LearningLanguage}
                        - Chủ đề: {flashcardSet.Title ?? "Không có tiêu đề"} (nếu trống, chọn ngẫu nhiên)
                        - Trình độ: {userLevel.Level} (từ 1 đến 6, với 1 là cơ bản và 6 là nâng cao)
                        - Số lượng: {50}


                         Mỗi flashcard có định dạng như sau:

                        **Flashcard 1:**
                        *   **Vintage Toy**
                        *   Noun /ˈvɪn.tɪdʒ tɔɪ/
                        *   Dịch nghĩa: Đồ chơi cổ điển
                        *   Ví dụ: ""The collector specialized in restoring vintage toys to their original condition.""
                        *   Dịch câu: ""Nhà sưu tập chuyên phục hồi những món đồ chơi cổ điển về tình trạng ban đầu.""

                       Yêu cầu: 
                        !!! Dịch nghĩa của từ vựng sang ngôn ngữ mẹ đẻ {flashcardSet.NativeLanguage}
                        !!! Dịch câu ví dụ sang ngôn ngữ mẹ đẻ {flashcardSet.NativeLanguage}
                        !!! Câu ví dụ phải dùng sử dụng từ vựng ngôn ngữ học {flashcardSet.LearningLanguage}

                        Lưu ý: 
                        - Thực hiện phản hồi đúng với yêu cầu
                        - Đảm bảo thông tin đúng và chính xác       
                        - Đảm bảo thông tin học thuật và phù hợp với trình độ
                        - Không thêm bất kỳ văn bản nào khác (kể cả chú thích)
                        - Vào luôn vấn đề chính không cần xác nhận lại câu hỏi của tôi , không thêm thông tin không cần thiết
                ";

                var apiRequestBuilder = new ApiRequestBuilder()
               .WithPrompt(prompt)
               .WithDefaultGenerationConfig(temperature: 0.75F)
               .Build();

                var modelVersion = ModelVersion.Gemini_20_Flash;

                var response = await generator.GenerateContentAsync(apiRequestBuilder, modelVersion);
                var flashcards = ParseFlashcards(response.Result, flcardSet.FlashcardSetId);

                // Create flashcards
                await _flashCardRepository.CreateRangeAsync(flcardSet.FlashcardSetId, flashcards);

                await trans.CommitAsync();

                return flcardSet;

            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
               
            }

        }

        public async Task<FlashcardSet?> DeleteByIdAsync(Guid id, string userId)
        {
            await using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Delete flcardSet and flcards
                var review = await _flashcardReviewRepository.DeleteAllReviewByFlashcardSetIdAsync(id, userId);
                if (review)
                {
                    var flcardSet = await _flashcardSetRepository.DeleteAsync(id, userId);
                    // Delete Review

                    await trans.CommitAsync();
                    return flcardSet;
                }
                
                await trans.CommitAsync();
                return null;
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        private List<Flashcard?> ParseFlashcards(string input, Guid flashcardSetId)
        {
            var flashcards = new List<Flashcard>();
            var TimeCreated = DateTime.UtcNow;
            // Tách từng flashcard dựa trên số thứ tự
            var cardRegex = new Regex(@"\*\*Flashcard (\d+):\*\*(.*?)(?=\*\*Flashcard \d+:\*\*|\Z)", RegexOptions.Singleline);
            var cardMatches = cardRegex.Matches(input);

            foreach (Match cardMatch in cardMatches)
            {
                if (!cardMatch.Success) continue;

                var cardText = cardMatch.Groups[2].Value.Trim();
                var flashcard = new Flashcard();
                

                // Lấy từ vựng (nằm sau ** và trước *)
                var wordMatch = Regex.Match(cardText, @"\*\*([^*]+)\*\*");
                if (wordMatch.Success)
                {
                    flashcard.Term = wordMatch.Groups[1].Value.Trim();
                }

                // Lấy loại từ và phát âm
                var typePronMatch = Regex.Match(cardText, @"\*   (.+?) /(.+?)/");
                if (typePronMatch.Success)
                {
                    flashcard.Definition = typePronMatch.Groups[1].Value.Trim();
                    flashcard.Definition += " /" + typePronMatch.Groups[2].Value.Trim() + "/ ";
                }

                // Lấy dịch nghĩa
                var transMatch = Regex.Match(cardText, @"Dịch nghĩa: (.+)");
                if (transMatch.Success)
                {
                    flashcard.Definition += transMatch.Groups[1].Value.Trim();
                }

                // Lấy ví dụ (xử lý cả trường hợp có dấu * trong câu)
                var exampleMatch = Regex.Match(cardText, @"Ví dụ: ""(.+?)""(?=\s*\*|$)");
                if (exampleMatch.Success)
                {
                    flashcard.Example = exampleMatch.Groups[1].Value.Trim() +" ";
                }

                // Lấy dịch câu ví dụ
                var exampleTransMatch = Regex.Match(cardText, @"Dịch câu: ""(.+?)""(?=\s*\*|$)");
                if (exampleTransMatch.Success)
                {
                    flashcard.Example += exampleTransMatch.Groups[1].Value.Trim();
                }
                flashcard.CreatedAt = TimeCreated;
                flashcard.FlashcardSetId = flashcardSetId;

                flashcards.Add(flashcard);
            }

            return flashcards;
        }



    }
}
