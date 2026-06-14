using Ubad.Models;

namespace Ubad.Services
{
    public interface ISettingsService
    {
        AppSettings Load();
        void        Save(AppSettings settings);
        void        ApplyTheme(string themeMode);
    }
}