using Ubad.ViewModels;

namespace Ubad.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _vm;

        public HomePage(HomeViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Animate hero
            HeroSection.Opacity      = 0;
            HeroSection.TranslationY = -20;

            await Task.WhenAll(
                HeroSection.FadeTo(1, 500, Easing.CubicOut),
                HeroSection.TranslateTo(0, 0, 500, Easing.CubicOut)
            );

            if (_vm.FeaturedRepos.Count == 0)
                await _vm.LoadAsync();
        }
    }
}