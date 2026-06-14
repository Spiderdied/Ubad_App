using System.Text.Json;
using Ubad.Configurations;

namespace Ubad.Services
{
    public class CacheService : ICacheService
    {
        private record CacheEntry<T>(T Data, DateTime ExpiresAt);

        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private string GetFullKey(string key) =>
            $"{AppConfig.CachePrefix}{key}";

        // ✅ يحذف البيانات التالفة أو المنتهية تلقائياً
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            await Task.Yield();
            var fullKey = GetFullKey(key);
            try
            {
                var raw = Preferences.Get(fullKey, string.Empty);
                if (string.IsNullOrWhiteSpace(raw)) return null;

                var entry = JsonSerializer.Deserialize<CacheEntry<T>>(raw, _opts);
                if (entry == null || entry.ExpiresAt < DateTime.UtcNow)
                {
                    Preferences.Remove(fullKey); // ✅ احذف المنتهية الصلاحية
                    return null;
                }
                return entry.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Cache] GetAsync error for '{key}': {ex.Message}");
                Preferences.Remove(fullKey); // ✅ احذف البيانات التالفة
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, int expiryMinutes) where T : class
        {
            await Task.Yield();
            try
            {
                var entry = new CacheEntry<T>(value,
                    DateTime.UtcNow.AddMinutes(expiryMinutes));
                var json = JsonSerializer.Serialize(entry, _opts);
                Preferences.Set(GetFullKey(key), json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Cache] SetAsync error for '{key}': {ex.Message}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Yield();
            Preferences.Remove(GetFullKey(key));
        }

        public async Task ClearAllAsync()
        {
            await Task.Yield();
            var knownKeys = new[]
            {
                "github_profile",
                "pinned_repos"
            };
            foreach (var k in knownKeys)
                Preferences.Remove(GetFullKey(k));
        }

        public async Task<long> GetCacheSizeBytesAsync()
        {
            await Task.Yield();
            long size = 0;
            var knownKeys = new[] { "github_profile", "pinned_repos" };
            foreach (var k in knownKeys)
            {
                var raw = Preferences.Get(GetFullKey(k), string.Empty);
                size += raw.Length * 2L; // UTF-16
            }
            return size;
        }
    }
}
