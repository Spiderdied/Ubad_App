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

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            await Task.Yield();
            try
            {
                var fullKey = GetFullKey(key);
                var raw     = Preferences.Get(fullKey, string.Empty);
                if (string.IsNullOrEmpty(raw)) return null;

                var entry = JsonSerializer.Deserialize<CacheEntry<T>>(raw, _opts);
                if (entry == null || entry.ExpiresAt < DateTime.UtcNow)
                {
                    Preferences.Remove(fullKey);
                    return null;
                }
                return entry.Data;
            }
            catch { return null; }
        }

        public async Task SetAsync<T>(string key, T value, int expiryMinutes) where T : class
        {
            await Task.Yield();
            try
            {
                var entry   = new CacheEntry<T>(value, DateTime.UtcNow.AddMinutes(expiryMinutes));
                var json    = JsonSerializer.Serialize(entry, _opts);
                Preferences.Set(GetFullKey(key), json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cache] Set error: {ex.Message}");
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
            // MAUI Preferences doesn't expose prefix-based clearing natively;
            // clear known keys.
            var keys = new[] { "github_profile", "pinned_repos" };
            foreach (var k in keys)
                Preferences.Remove(GetFullKey(k));
        }

        public async Task<long> GetCacheSizeBytesAsync()
        {
            await Task.Yield();
            // Approximate: sum stored string lengths * 2 (UTF-16)
            long size = 0;
            var keys  = new[] { "github_profile", "pinned_repos" };
            foreach (var k in keys)
            {
                var raw = Preferences.Get(GetFullKey(k), string.Empty);
                size += raw.Length * 2;
            }
            return size;
        }
    }
}