using Ubad.ViewModels;

namespace Ubad.Views
{
    public partial class SplashPage : ContentPage
    {
        private readonly SplashViewModel _vm;

        public SplashPage(SplashViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RunAnimationsAsync();
        }

        private async Task RunAnimationsAsync()
        {
            // Reset initial states
            LogoBorder.Scale        = 0.5;
            LogoBorder.Opacity      = 0;
            AppNameLabel.Opacity    = 0;
            AppNameLabel.TranslationY = 20;
            TaglineLabel.Opacity    = 0;
            TaglineLabel.TranslationY = 15;
            DotsContainer.Opacity   = 0;

            // Animate logo
            await Task.WhenAll(
                LogoBorder.FadeTo(1, 600, Easing.CubicOut),
                LogoBorder.ScaleTo(1, 600, Easing.CubicOut)
            );

            await Task.Delay(100);

            // Animate app name
            await Task.WhenAll(
                AppNameLabel.FadeTo(1, 400, Easing.CubicOut),
                AppNameLabel.TranslateTo(0, 0, 400, Easing.CubicOut)
            );

            await Task.Delay(80);

            // Animate tagline
            await Task.WhenAll(
                TaglineLabel.FadeTo(1, 350, Easing.CubicOut),
                TaglineLabel.TranslateTo(0, 0, 350, Easing.CubicOut)
            );

            // Show loading dots
            await DotsContainer.FadeTo(1, 300);

            // Start dot pulse animation
            var dotsCts = new CancellationTokenSource();
            _ = AnimateDotsAsync(dotsCts.Token);

            // Pre-warm data in background
            await _vm.InitializeAsync();

            // Stop dots and fade out
            dotsCts.Cancel();

            await Task.WhenAll(
                LogoStack.FadeTo(0, 300, Easing.CubicIn),
                DotsContainer.FadeTo(0, 300)
            );

            // ✅ Navigate via DI — لا new AppShell()
            var shell = IPlatformApplication.Current!.Services
                            .GetRequiredService<AppShell>();

            Application.Current!.MainPage = shell;
        }

        private async Task AnimateDotsAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await Dot1.ScaleTo(1.6, 200, Easing.CubicOut);
                    if (ct.IsCancellationRequested) break;
                    await Dot1.ScaleTo(1.0, 200, Easing.CubicIn);

                    await Dot2.ScaleTo(1.6, 200, Easing.CubicOut);
                    if (ct.IsCancellationRequested) break;
                    await Dot2.ScaleTo(1.0, 200, Easing.CubicIn);

                    await Dot3.ScaleTo(1.6, 200, Easing.CubicOut);
                    if (ct.IsCancellationRequested) break;
                    await Dot3.ScaleTo(1.0, 200, Easing.CubicIn);

                    await Task.Delay(300, ct);
                }
            }
            catch (TaskCanceledException) { /* طبيعي */ }
        }
    }
}
