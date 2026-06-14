using System.Windows.Input;
using Ubad.Models;

namespace Ubad.Components
{
    public partial class ProjectCard : Border
    {
        // ── Bindable Properties ───────────────────────────────────

        public static readonly BindableProperty RepoProperty =
            BindableProperty.Create(nameof(Repo), typeof(GitHubRepository),
                typeof(ProjectCard), null, propertyChanged: OnRepoChanged);

        public static readonly BindableProperty TapCommandProperty =
            BindableProperty.Create(nameof(TapCommand), typeof(ICommand),
                typeof(ProjectCard), null);

        public static readonly BindableProperty FavoriteCommandProperty =
            BindableProperty.Create(nameof(FavoriteCommand), typeof(ICommand),
                typeof(ProjectCard), null);

        // ── Properties ────────────────────────────────────────────

        public GitHubRepository? Repo
        {
            get => (GitHubRepository?)GetValue(RepoProperty);
            set => SetValue(RepoProperty, value);
        }

        public ICommand? TapCommand
        {
            get => (ICommand?)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public ICommand? FavoriteCommand
        {
            get => (ICommand?)GetValue(FavoriteCommandProperty);
            set => SetValue(FavoriteCommandProperty, value);
        }

        // ── Constructor ───────────────────────────────────────────

        public ProjectCard()
        {
            InitializeComponent();
        }

        // ── Callbacks ─────────────────────────────────────────────

        private static void OnRepoChanged(BindableObject bindable, object oldVal, object newVal)
        {
            if (bindable is ProjectCard card && newVal is GitHubRepository repo)
                card.BindingContext = repo;
        }

        // ── Event Handlers ────────────────────────────────────────

        private async void OnCardTapped(object sender, TappedEventArgs e)
        {
            // Tap animation
            await this.ScaleTo(0.97, 80, Easing.CubicOut);
            await this.ScaleTo(1.0,  80, Easing.CubicIn);

            if (Repo != null && TapCommand?.CanExecute(Repo) == true)
                TapCommand.Execute(Repo);
        }

        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (Repo == null) return;

            // Heart animation
            await FavoriteButton.ScaleTo(1.4, 100, Easing.CubicOut);
            await FavoriteButton.ScaleTo(1.0, 100, Easing.CubicIn);

            if (FavoriteCommand?.CanExecute(Repo) == true)
                FavoriteCommand.Execute(Repo);
        }

        private async void OnGitHubClicked(object sender, EventArgs e)
        {
            if (Repo == null) return;
            await Browser.OpenAsync(Repo.Url, BrowserLaunchMode.SystemPreferred);
        }

        private async void OnWebsiteClicked(object sender, EventArgs e)
        {
            if (Repo?.GitHubPagesUrl == null) return;
            await Browser.OpenAsync(Repo.GitHubPagesUrl, BrowserLaunchMode.SystemPreferred);
        }
    }
}