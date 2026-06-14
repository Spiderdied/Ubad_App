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
            // Logo fade + scale
            LogoBorder.Scale   = 0.5;
            LogoBorder.Opacity = 0;

            await Task.WhenAll(
                LogoBorder.FadeTo(1, 600, Easing.CubicOut),
                LogoBorder.ScaleTo(1, 600, Easing.CubicOut)
            );

            await Task.Delay(100);

            // App name slide up
            AppNameLabel.TranslationY = 20;
            await Task.WhenAll(
                AppNameLabel.FadeTo(1, 400, Easing.CubicOut),
                AppNameLabel.TranslateTo(0, 0, 400, Easing.CubicOut)
            );

            await Task.Delay(80);

            TaglineLabel.TranslationY = 15;
            await Task.WhenAll(
                TaglineLabel.FadeTo(1, 350, Easing.CubicOut),
                TaglineLabel.TranslateTo(0, 0, 350, Easing.CubicOut)
            );

            // Dots
            await DotsContainer.FadeTo(1, 300);

            // Pulse dots
            _ = AnimateDotsAsync();

            // Initialize app (pre-warm)
            await _vm.InitializeAsync();

            // Transition
            await Task.WhenAll(
                LogoStack.FadeTo(0, 300, Easing.CubicIn),
                DotsContainer.FadeTo(0, 300)
            );

            // Navigate to shell
            Application.Current!.MainPage = new AppShell();
        }

        private async Task AnimateDotsAsync()
        {
            while (IsVisible)
            {
                await Dot1.ScaleTo(1.6, 200, Easing.CubicOut);
                await Dot1.ScaleTo(1.0, 200, Easing.CubicIn);
                await Dot2.ScaleTo(1.6, 200, Easing.CubicOut);
                await Dot2.ScaleTo(1.0, 200, Easing.CubicIn);
                await Dot3.ScaleTo(1.6, 200, Easing.CubicOut);
                await Dot3.ScaleTo(1.0, 200, Easing.CubicIn);
                await Task.Delay(300);
            }
        }
    }
}