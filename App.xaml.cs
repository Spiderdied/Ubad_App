using Ubad.Services;
using Ubad.Views;

namespace Ubad
{
    public partial class App : Application
    {
        public App(ISettingsService settings)
        {
            InitializeComponent();

            // Apply theme before any UI renders
            var s = settings.Load();
            UserAppTheme = s.ThemeMode switch
            {
                "Light" => AppTheme.Light,
                "Dark"  => AppTheme.Dark,
                _       => AppTheme.Unspecified
            };

            MainPage = new SplashPage(
                Handler?.MauiContext?.Services.GetService<ViewModels.SplashViewModel>()!
            );
        }
    }
}