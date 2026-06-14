using System.Windows.Input;
using Ubad.Configurations;
using Ubad.Models;
using Ubad.Services;

namespace Ubad.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settings;
        private readonly ICacheService    _cache;

        private AppSettings _appSettings = new();

        // ── Theme ─────────────────────────────────────────────────

        private string _themeMode = "System";
        public string ThemeMode
        {
            get => _themeMode;
            set
            {
                SetProperty(ref _themeMode, value);
                _appSettings.ThemeMode = value;
                _settings.ApplyTheme(value);
                SaveSettings();
            }
        }

        public bool IsLightTheme
        {
            get => ThemeMode == "Light";
            set { if (value) ThemeMode = "Light"; }
        }
        public bool IsDarkTheme
        {
            get => ThemeMode == "Dark";
            set { if (value) ThemeMode = "Dark"; }
        }
        public bool IsSystemTheme
        {
            get => ThemeMode == "System";
            set { if (value) ThemeMode = "System"; }
        }

        // ── Animations ────────────────────────────────────────────

        private bool _enableAnimations = true;
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set
            {
                SetProperty(ref _enableAnimations, value);
                _appSettings.EnableAnimations = value;
                SaveSettings();
            }
        }

        // ── Cache ─────────────────────────────────────────────────

        private bool _enableCache = true;
        public bool EnableCache
        {
            get => _enableCache;
            set
            {
                SetProperty(ref _enableCache, value);
                _appSettings.EnableCache = value;
                SaveSettings();
            }
        }

        private string _cacheSizeDisplay = "Calculating…";
        public string CacheSizeDisplay
        {
            get => _cacheSizeDisplay;
            set => SetProperty(ref _cacheSizeDisplay, value);
        }

        // ── App Info ──────────────────────────────────────────────

        public string AppVersion  => AppConfig.AppVersion;
        public string AppName     => AppConfig.AppName;
        public string GitHubUser  => AppConfig.GitHubUsername;

        // ── Commands ──────────────────────────────────────────────

        public ICommand LoadCommand       { get; }
        public ICommand ClearCacheCommand { get; }
        public ICommand OpenGitHubCommand { get; }
        public ICommand RateAppCommand    { get; }

        public SettingsViewModel(ISettingsService settings, ICacheService cache)
        {
            _settings = settings;
            _cache    = cache;

            LoadCommand       = CreateCommand(LoadAsync);
            ClearCacheCommand = CreateCommand(ClearCacheAsync);
            OpenGitHubCommand = CreateCommand(OpenGitHubAsync);
            RateAppCommand    = CreateCommand(RateAppAsync);
        }

        public async Task LoadAsync()
        {
            _appSettings = _settings.Load();

            _themeMode        = _appSettings.ThemeMode;
            _enableAnimations = _appSettings.EnableAnimations;
            _enableCache      = _appSettings.EnableCache;

            OnPropertyChanged(nameof(ThemeMode));
            OnPropertyChanged(nameof(IsLightTheme));
            OnPropertyChanged(nameof(IsDarkTheme));
            OnPropertyChanged(nameof(IsSystemTheme));
            OnPropertyChanged(nameof(EnableAnimations));
            OnPropertyChanged(nameof(EnableCache));

            await UpdateCacheSizeAsync();
        }

        private async Task UpdateCacheSizeAsync()
        {
            var bytes = await _cache.GetCacheSizeBytesAsync();
            CacheSizeDisplay = bytes switch
            {
                < 1024              => $"{bytes} B",
                < 1024 * 1024       => $"{bytes / 1024.0:F1} KB",
                _                   => $"{bytes / (1024.0 * 1024):F1} MB"
            };
        }

        private async Task ClearCacheAsync()
        {
            await _cache.ClearAllAsync();
            CacheSizeDisplay = "0 B";
        }

        private async Task OpenGitHubAsync() =>
            await Browser.OpenAsync(AppConfig.GitHubProfileUrl,
                BrowserLaunchMode.SystemPreferred);

        private async Task RateAppAsync() =>
            await Browser.OpenAsync(AppConfig.GitHubProfileUrl,
                BrowserLaunchMode.SystemPreferred);

        private void SaveSettings() => _settings.Save(_appSettings);
    }
}