namespace Ubad.Services
{
    public interface ICacheService
    {
        Task<T?>  GetAsync<T>(string key) where T : class;
        Task      SetAsync<T>(string key, T value, int expiryMinutes) where T : class;
        Task      RemoveAsync(string key);
        Task      ClearAllAsync();
        Task<long> GetCacheSizeBytesAsync();
    }
}