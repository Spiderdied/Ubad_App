using Ubad.Services;

namespace Ubad.ViewModels
{
    public class SplashViewModel : BaseViewModel
    {
        private readonly IGitHubService  _github;
        private readonly ISettingsService _settings;

        public SplashViewModel(IGitHubService github, ISettingsService settings)
        {
            _github   = github;
            _settings = settings;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Apply saved theme immediately
                var s = _settings.Load();
                _settings.ApplyTheme(s.ThemeMode);

                // Pre-warm cache in background
                _ = Task.Run(() => _github.GetProfileAsync());
                _ = Task.Run(() => _github.GetPinnedRepositoriesAsync());

                // Minimum splash duration
                await Task.Delay(2200);
            }
            catch
            {
                // Splash must always navigate forward
            }
        }
    }
}