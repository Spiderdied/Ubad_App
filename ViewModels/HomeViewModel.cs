using System.Collections.ObjectModel;
using System.Windows.Input;
using Ubad.Models;
using Ubad.Services;

namespace Ubad.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IGitHubService    _github;
        private readonly IFavoritesService _favorites;

        // ── Observable State ──────────────────────────────────────

        private GitHubProfile? _profile;
        public GitHubProfile? Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        public ObservableCollection<GitHubRepository> FeaturedRepos { get; } = new();

        private int _totalStars;
        public int TotalStars
        {
            get => _totalStars;
            set => SetProperty(ref _totalStars, value);
        }

        private int _totalForks;
        public int TotalForks
        {
            get => _totalForks;
            set => SetProperty(ref _totalForks, value);
        }

        private string _greeting = "Welcome back";
        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value);
        }

        // ── Commands ──────────────────────────────────────────────

        public ICommand LoadCommand      { get; }
        public ICommand RefreshCommand   { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand ViewAllCommand   { get; }
        public ICommand NavigateToDetailCommand { get; }

        public HomeViewModel(IGitHubService github, IFavoritesService favorites)
        {
            _github    = github;
            _favorites = favorites;

            LoadCommand             = CreateCommand(LoadAsync);
            RefreshCommand          = CreateCommand(RefreshAsync);
            OpenProfileCommand      = CreateCommand(OpenProfileAsync);
            ViewAllCommand          = CreateCommand(ViewAllAsync);
            NavigateToDetailCommand = CreateCommand<GitHubRepository>(NavigateToDetailAsync);

            SetGreeting();
        }

        private void SetGreeting()
        {
            Greeting = DateTime.Now.Hour switch
            {
                < 12 => "Good morning ☀️",
                < 17 => "Good afternoon 🌤️",
                _    => "Good evening 🌙"
            };
        }

        public async Task LoadAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            ClearState();

            try
            {
                var profileTask = _github.GetProfileAsync();
                var reposTask   = _github.GetPinnedRepositoriesAsync();

                await Task.WhenAll(profileTask, reposTask);

                Profile = await profileTask;

                var repos = await reposTask;
                FeaturedRepos.Clear();

                foreach (var r in repos.Take(3))
                {
                    r.IsFavorite = await _favorites.IsFavoriteAsync(r.Id);
                    FeaturedRepos.Add(r);
                }

                if (Profile != null)
                    Profile.PinnedReposCount = repos.Count;

                TotalStars = repos.Sum(r => r.Stars);
                TotalForks = repos.Sum(r => r.Forks);

                IsEmpty = FeaturedRepos.Count == 0;
            }
            catch (Exception ex)
            {
                SetError($"Failed to load profile.\n{ex.Message}");
            }
            finally
            {
                IsLoading = false;
                UpdateShowContent();
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadAsync();
            IsRefreshing = false;
        }

        private async Task OpenProfileAsync() =>
            await Browser.OpenAsync(
                $"https://github.com/{Configurations.AppConfig.GitHubUsername}",
                BrowserLaunchMode.SystemPreferred);

        private async Task ViewAllAsync() =>
            await NavigateToAsync("//projects");

        private async Task NavigateToDetailAsync(GitHubRepository repo) =>
            await Shell.Current.GoToAsync("projectdetail",
                new Dictionary<string, object> { ["Repo"] = repo });
    }
}