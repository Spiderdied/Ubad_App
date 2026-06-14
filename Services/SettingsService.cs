using System.Text.Json;
using Ubad.Models;

namespace Ubad.Services
{
    public class SettingsService : ISettingsService
    {
        private const string Key = "ubad_settings";

        private static readonly JsonSerializerOptions _opts = new()
        { PropertyNameCaseInsensitive = true };

        public AppSettings Load()
        {
            var raw = Preferences.Get(Key, string.Empty);
            if (string.IsNullOrEmpty(raw)) return new AppSettings();
            try   { return JsonSerializer.Deserialize<AppSettings>(raw, _opts) ?? new(); }
            catch { return new AppSettings(); }
        }

        public void Save(AppSettings s) =>
            Preferences.Set(Key, JsonSerializer.Serialize(s, _opts));

        public void ApplyTheme(string themeMode)
        {
            Application.Current!.UserAppTheme = themeMode switch
            {
                "Light"  => AppTheme.Light,
                "Dark"   => AppTheme.Dark,
                _        => AppTheme.Unspecified
            };
        }
    }
}