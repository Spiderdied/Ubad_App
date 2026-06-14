using Ubad.Services;
using Ubad.Views;

namespace Ubad
{
    public partial class App : Application
    {
        private readonly ISettingsService _settings;
        private readonly IServiceProvider _services;

        public App(ISettingsService settings, IServiceProvider services)
        {
            InitializeComponent();
            _settings = settings;
            _services = services;

            var s = _settings.Load();
            UserAppTheme = s.ThemeMode switch
            {
                "Light" => AppTheme.Light,
                "Dark"  => AppTheme.Dark,
                _       => AppTheme.Unspecified
            };

            MainPage = _services.GetRequiredService<SplashPage>();
        }
    }
}
