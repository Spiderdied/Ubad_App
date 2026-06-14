using System.Windows.Input;
using Ubad.Models;
using Ubad.Services;

namespace Ubad.ViewModels
{
    [QueryProperty(nameof(Repo), "Repo")]
    public class ProjectDetailViewModel : BaseViewModel
    {
        private readonly IFavoritesService _favorites;
        private readonly IGitHubService    _github;

        // ── Repo ──────────────────────────────────────────────────

        private GitHubRepository? _repo;
        public GitHubRepository? Repo
        {
            get => _repo;
            set
            {
                SetProperty(ref _repo, value);
                if (value != null)
                    _ = InitAsync(value);
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        // ── Commands ──────────────────────────────────────────────

        public ICommand GoBackCommand          { get; }
        public ICommand ToggleFavoriteCommand  { get; }
        public ICommand OpenGitHubCommand      { get; }
        public ICommand OpenWebsiteCommand     { get; }
        public ICommand OpenBrowserCommand     { get; }
        public ICommand ShareCommand           { get; }
        public ICommand CopyUrlCommand         { get; }
        public ICommand CopyPagesUrlCommand    { get; }

        public ProjectDetailViewModel(IFavoritesService favorites, IGitHubService github)
        {
            _favorites = favorites;
            _github    = github;

            GoBackCommand         = CreateCommand(NavigateBackAsync);
            ToggleFavoriteCommand = CreateCommand(ToggleFavoriteAsync);
            OpenGitHubCommand     = CreateCommand(OpenGitHubAsync);
            OpenWebsiteCommand    = CreateCommand(OpenWebsiteAsync);
            OpenBrowserCommand    = CreateCommand<string>(OpenEmbeddedBrowserAsync);
            ShareCommand          = CreateCommand(ShareAsync);
            CopyUrlCommand        = CreateCommand(CopyUrlAsync);
            CopyPagesUrlCommand   = CreateCommand(CopyPagesUrlAsync);
        }

        private async Task InitAsync(GitHubRepository repo)
        {
            IsFavorite = await _favorites.IsFavoriteAsync(repo.Id);
        }

        private async Task ToggleFavoriteAsync()
        {
            if (_repo == null) return;
            await _favorites.ToggleAsync(_repo);
            IsFavorite = _repo.IsFavorite;
        }

        private async Task OpenGitHubAsync()
        {
            if (_repo == null) return;
            await Browser.OpenAsync(_repo.Url, BrowserLaunchMode.SystemPreferred);
        }

        private async Task OpenWebsiteAsync()
        {
            if (_repo?.GitHubPagesUrl == null) return;
            await Browser.OpenAsync(_repo.GitHubPagesUrl, BrowserLaunchMode.SystemPreferred);
        }

        private async Task OpenEmbeddedBrowserAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            await Shell.Current.GoToAsync("browser",
                new Dictionary<string, object> { ["Url"] = url });
        }

        private async Task ShareAsync()
        {
            if (_repo == null) return;
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = _repo.Name,
                Text  = $"Check out {_repo.Name}!\n{_repo.Url}"
            });
        }

        private async Task CopyUrlAsync()
        {
            if (_repo == null) return;
            await Clipboard.Default.SetTextAsync(_repo.Url);
            await Shell.Current.DisplayAlert("Copied", "Repository URL copied!", "OK");
        }

        private async Task CopyPagesUrlAsync()
        {
            if (_repo?.GitHubPagesUrl == null) return;
            await Clipboard.Default.SetTextAsync(_repo.GitHubPagesUrl);
            await Shell.Current.DisplayAlert("Copied", "Website URL copied!", "OK");
        }
    }
}