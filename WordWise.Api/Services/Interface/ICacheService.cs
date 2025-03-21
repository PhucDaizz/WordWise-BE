namespace WordWise.Api.Services.Interface
{
    public interface ICacheService
    {
        void StoreApiKey(string userId, string apiKey);
        bool TryGetApiKey(string userId, out string apiKey);
    }
}
