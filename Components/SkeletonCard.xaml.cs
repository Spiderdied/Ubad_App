namespace Ubad.Components
{
    public partial class SkeletonCard : Border
    {
        public SkeletonCard()
        {
            InitializeComponent();
            AnimateShimmer();
        }

        private async void AnimateShimmer()
        {
            while (true)
            {
                await this.FadeTo(0.5, 800, Easing.CubicInOut);
                await this.FadeTo(1.0, 800, Easing.CubicInOut);
            }
        }
    }
}