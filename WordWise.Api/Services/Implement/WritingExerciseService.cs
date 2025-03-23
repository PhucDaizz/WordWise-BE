using Gemini.NET;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using WordWise.Api.Data;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class WritingExerciseService : IWritingExerciseService
    {
        private readonly IWritingExerciseRepository _writingExerciseRepository;
        private readonly WordWiseDbContext dbContext;
        private readonly ICacheService _cacheService;

        public WritingExerciseService(IWritingExerciseRepository writingExerciseRepository, WordWiseDbContext dbContext, ICacheService cacheService)
        {
            _writingExerciseRepository = writingExerciseRepository;
            _cacheService = cacheService;
            this.dbContext = dbContext;
        }

        public async Task<string> GetFeedBackFromAi(Guid writingExerciseId)
        {
            // Get Content
            var writingExercise = await _writingExerciseRepository.GetByIdAsync(writingExerciseId);
            if (writingExercise == null)
            {
                throw new ArgumentException($"Writing exercise with ID {writingExerciseId} not found.");
            }

            // Validate Writing Exercise Fields
            if (string.IsNullOrEmpty(writingExercise.Content) ||
                string.IsNullOrEmpty(writingExercise.Topic) ||
                string.IsNullOrEmpty(writingExercise.NativeLanguage) ||
                string.IsNullOrEmpty(writingExercise.LearningLanguage))
            {
                throw new ArgumentException("Invalid writing exercise data. Please ensure all required fields are filled.");
            }

            // Retrieve API Key
            var userId = writingExercise.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId is missing for the writing exercise.");
            }

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

            string prompt = 
                $@"Bạn là một trợ lý Genz với vibe tuổi teen siêu cool! chuyên đánh giá và phản hồi về các bài viết, đặc biệt chú trọng vào việc hỗ trợ người học ngôn ngữ.Hãy phân tích bài viết người dùng cung cấp và đưa ra phản hồi chi tiết, hữu ích, và dí dỏm đừng quá máy móc tập trung vào các khía cạnh sau:
                
                * *Thông tin bài viết: **
                    *Chủ đề bài viết: { writingExercise.Topic}
                    *Nội dung bài viết: { writingExercise.Content} (cần bạn kiểm tra nếu bài biết không được viết đúng ngôn ngữ {writingExercise.LearningLanguage} được xem là sai đề)
                    *Ngôn ngữ gốc của người viết: { writingExercise.NativeLanguage}
                    *Ngôn ngữ đang học: { writingExercise.LearningLanguage}

                1.  * *Phân tích bài viết: **
                    *Đưa ra số điểm trên thang điểm 100 của bài viết
                    *Đánh giá tính liên quan của nội dung bài viết đến chủ đề '{writingExercise.Topic}'.
                    *Đánh giá cấu trúc tổ chức bài viết(mở đầu, thân bài, kết luận).
                    *Đánh giá chi tiết tính chính xác của ngữ pháp, chính tả và lựa chọn từ ngữ trong { writingExercise.LearningLanguage} (chi tiết nhất có thể).
                    *Đánh giá phong cách viết(sự hấp dẫn, sử dụng ví dụ hoặc hình ảnh minh họa).
                    *Đánh giá tính độc đáo và sáng tạo của ý tưởng về '{writingExercise.Topic}'.
                    (!!!Lưu ý: Khi chỉnh lại câu viết của bài thì bắt buộc phải viết nghiêm túc)

                2.  * *Phản hồi chi tiết: **
                    *Nêu rõ những điểm mạnh của người liên quan đến '{writingExercise.Topic}' trong { writingExercise.LearningLanguage}.
                    *Đưa ra các lĩnh vực cần cải thiện và gợi ý cụ thể để nâng cao kỹ năng sử dụng { writingExercise.LearningLanguage}.
                    *Đảm bảo phản hồi rõ ràng, hữu ích và mang tính xây dựng.

                **Lưu ý quan trọng: **
                    *Viết phản hồi hoàn toàn bằng { writingExercise.NativeLanguage} và đi thằng vào vấn đề không cần chào hỏi.
                    *Nếu bài viết dùng khác ngôn ngữ {writingExercise.LearningLanguage} được xem là sai đề.
                    *Đảm bảo phản hồi tự nhiên, chính xác và phù hợp với { writingExercise.NativeLanguage}.
                    *Duy trì giọng điệu tuổi teen, năng động, hợp trend, dí dỏm, sến , tích cực trong phản hồi.
                    *Tập trung vào việc giúp người viết cải thiện bài viết về '{writingExercise.Topic}' và kỹ năng sử dụng { writingExercise.LearningLanguage} và tập luyện nhiều trên app WordWide của chúng tôi có các tính năng học flashcard, luyện viết, trắc nghiệm bài đọc, kho từ điển xịn sò.";

            var apiRequestBuilder = new ApiRequestBuilder()
                .WithSystemInstruction("Bạn là một trợ lý Genz với vibe tuổi teen siêu cool! và có rất nhiều kinh nghiệm tâm huyết với nghề về làm bài tập luyện viết top 1 thế giới khi đưa ra chỉnh sửa bài thì nghiêm túc")
                .WithPrompt(prompt)
                .WithDefaultGenerationConfig(temperature: 0.9F)
                .Build();

            var modelVersion = ModelVersion.Gemini_20_Flash;

            try
            {
                // Call AI Service and Generate Feedback
                var response = await generator.GenerateContentAsync(apiRequestBuilder, modelVersion);

                if (string.IsNullOrEmpty(response.Result))
                {
                    throw new InvalidOperationException("Failed to generate feedback from Gemini AI.");
                }

                // Save Feedback in Repository
                await _writingExerciseRepository.UpdateFeedback(writingExerciseId, response.Result);

                // Return Feedback
                return response.Result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while processing the feedback.");
            }

        }
    }
}
