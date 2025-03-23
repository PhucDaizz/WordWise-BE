using Microsoft.Extensions.Caching.Memory;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public void StoreApiKey(string userId, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("UserId và ApiKey không được để trống.");
            }

            string cacheKey = $"GeminiApiKey_{userId}";

            // Lưu API key (ghi đè nếu đã tồn tại)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            };
            memoryCache.Set(cacheKey, apiKey, cacheOptions);
        }

        public bool TryGetApiKey(string userId, out string apiKey)
        {
            string cacheKey = $"GeminiApiKey_{userId}";
            return this.memoryCache.TryGetValue(cacheKey, out apiKey);
        }
    }
}
