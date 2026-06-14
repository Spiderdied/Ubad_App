using Microsoft.Extensions.Logging;
using Ubad.Services;
using Ubad.ViewModels;
using Ubad.Views;

namespace Ubad
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-Regular.ttf",  "InterRegular");
                    fonts.AddFont("Inter-Medium.ttf",   "InterMedium");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter-Bold.ttf",     "InterBold");
                });

            // ── HTTP Client ───────────────────────────────────────
            builder.Services.AddHttpClient<GitHubService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddSingleton<IGitHubService>(sp =>
                sp.GetRequiredService<GitHubService>());

            // ── Services ──────────────────────────────────────────
            builder.Services.AddSingleton<ICacheService,     CacheService>();
            builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
            builder.Services.AddSingleton<ISettingsService,  SettingsService>();

            // ── Shell ─────────────────────────────────────────────
            builder.Services.AddSingleton<AppShell>();

            // ── ViewModels ────────────────────────────────────────
            builder.Services.AddTransient<SplashViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ProjectsViewModel>();
            builder.Services.AddTransient<ProjectDetailViewModel>();
            builder.Services.AddTransient<BrowserViewModel>();
            builder.Services.AddTransient<FavoritesViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // ── Views ─────────────────────────────────────────────
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<ProjectsPage>();
            builder.Services.AddSingleton<FavoritesPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddTransient<ProjectDetailPage>();
            builder.Services.AddTransient<BrowserPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
