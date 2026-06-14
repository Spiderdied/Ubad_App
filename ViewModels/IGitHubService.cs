using System.Collections.ObjectModel;
using System.Windows.Input;
using Ubad.Models;
using Ubad.Services;

namespace Ubad.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        private readonly IGitHubService    _github;
        private readonly IFavoritesService _favorites;

        // ── Data ──────────────────────────────────────────────────

        private List<GitHubRepository> _allRepos = new();

        public ObservableCollection<GitHubRepository> FilteredRepos { get; } = new();

        // ── Search ────────────────────────────────────────────────

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                ApplyFilters();
            }
        }

        // ── Sorting ───────────────────────────────────────────────

        private string _sortBy = "Stars";
        public string SortBy
        {
            get => _sortBy;
            set
            {
                SetProperty(ref _sortBy, value);
                ApplyFilters();
            }
        }

        // ── Filter ────────────────────────────────────────────────

        private string _languageFilter = "All";
        public string LanguageFilter
        {
            get => _languageFilter;
            set
            {
                SetProperty(ref _languageFilter, value);
                ApplyFilters();
            }
        }

        public List<string> AvailableLanguages { get; private set; } = new() { "All" };

        // ── Commands ──────────────────────────────────────────────

        public ICommand LoadCommand             { get; }
        public ICommand RefreshCommand          { get; }
        public ICommand ToggleFavoriteCommand   { get; }
        public ICommand NavigateToDetailCommand { get; }
        public ICommand OpenInBrowserCommand    { get; }
        public ICommand ShareCommand            { get; }
        public ICommand ClearSearchCommand      { get; }

        public ProjectsViewModel(IGitHubService github, IFavoritesService favorites)
        {
            _github    = github;
            _favorites = favorites;

            LoadCommand             = CreateCommand(LoadAsync);
            RefreshCommand          = CreateCommand(RefreshAsync);
            ToggleFavoriteCommand   = CreateCommand<GitHubRepository>(ToggleFavoriteAsync);
            NavigateToDetailCommand = CreateCommand<GitHubRepository>(NavigateToDetailAsync);
            OpenInBrowserCommand    = CreateCommand<GitHubRepository>(OpenInBrowserAsync);
            ShareCommand            = CreateCommand<GitHubRepository>(ShareAsync);
            ClearSearchCommand      = CreateCommand(ClearSearchAsync);
        }

        public async Task LoadAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            ClearState();

            try
            {
                _allRepos = await _github.GetPinnedRepositoriesAsync();

                foreach (var r in _allRepos)
                    r.IsFavorite = await _favorites.IsFavoriteAsync(r.Id);

                // Build language list
                var langs = _allRepos
                    .Where(r => !string.IsNullOrEmpty(r.PrimaryLanguage))
                    .Select(r => r.PrimaryLanguage)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToList();

                AvailableLanguages = new List<string> { "All" };
                AvailableLanguages.AddRange(langs);
                OnPropertyChanged(nameof(AvailableLanguages));

                ApplyFilters();
                IsEmpty = FilteredRepos.Count == 0;
            }
            catch (Exception ex)
            {
                SetError($"Could not load projects.\n{ex.Message}");
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

        private void ApplyFilters()
        {
            var result = _allRepos.AsEnumerable();

            // Search
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLowerInvariant();
                result = result.Where(r =>
                    r.Name.ToLower().Contains(q) ||
                    r.Description.ToLower().Contains(q) ||
                    r.PrimaryLanguage.ToLower().Contains(q));
            }

            // Language filter
            if (LanguageFilter != "All")
                result = result.Where(r => r.PrimaryLanguage == LanguageFilter);

            // Sort
            result = SortBy switch
            {
                "Stars"   => result.OrderByDescending(r => r.Stars),
                "Name"    => result.OrderBy(r => r.Name),
                "Updated" => result.OrderByDescending(r => r.UpdatedAt),
                "Forks"   => result.OrderByDescending(r => r.Forks),
                _         => result.OrderByDescending(r => r.Stars)
            };

            FilteredRepos.Clear();
            foreach (var r in result)
                FilteredRepos.Add(r);

            IsEmpty = FilteredRepos.Count == 0 && !IsLoading;
            UpdateShowContent();
        }

        private async Task ToggleFavoriteAsync(GitHubRepository repo)
        {
            await _favorites.ToggleAsync(repo);
            OnPropertyChanged(nameof(FilteredRepos));
        }

        private async Task NavigateToDetailAsync(GitHubRepository repo) =>
            await Shell.Current.GoToAsync("projectdetail",
                new Dictionary<string, object> { ["Repo"] = repo });

        private async Task OpenInBrowserAsync(GitHubRepository repo) =>
            await Browser.OpenAsync(repo.Url, BrowserLaunchMode.SystemPreferred);

        private async Task ShareAsync(GitHubRepository repo) =>
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = repo.Name,
                Text  = $"Check out {repo.Name} on GitHub!\n{repo.Url}"
            });

        private async Task ClearSearchAsync()
        {
            SearchQuery = string.Empty;
            await Task.CompletedTask;
        }
    }
}