using System.Collections.ObjectModel;
using System.Windows.Input;
using Ubad.Models;
using Ubad.Services;

namespace Ubad.ViewModels
{
    public class FavoritesViewModel : BaseViewModel
    {
        private readonly IFavoritesService _favorites;

        public ObservableCollection<FavoriteProject> Favorites { get; } = new();

        public ICommand LoadCommand          { get; }
        public ICommand RemoveCommand        { get; }
        public ICommand OpenGitHubCommand    { get; }
        public ICommand OpenWebsiteCommand   { get; }
        public ICommand ShareCommand         { get; }

        public FavoritesViewModel(IFavoritesService favorites)
        {
            _favorites = favorites;

            LoadCommand        = CreateCommand(LoadAsync);
            RemoveCommand      = CreateCommand<FavoriteProject>(RemoveAsync);
            OpenGitHubCommand  = CreateCommand<FavoriteProject>(OpenGitHubAsync);
            OpenWebsiteCommand = CreateCommand<FavoriteProject>(OpenWebsiteAsync);
            ShareCommand       = CreateCommand<FavoriteProject>(ShareAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            ClearState();

            try
            {
                var list = await _favorites.GetAllAsync();
                Favorites.Clear();

                foreach (var f in list.OrderByDescending(f => f.SavedAt))
                    Favorites.Add(f);

                IsEmpty = Favorites.Count == 0;
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            finally
            {
                IsLoading = false;
                UpdateShowContent();
            }
        }

        private async Task RemoveAsync(FavoriteProject fav)
        {
            await _favorites.RemoveAsync(fav.RepositoryId);
            Favorites.Remove(fav);
            IsEmpty = Favorites.Count == 0;
            UpdateShowContent();
        }

        private async Task OpenGitHubAsync(FavoriteProject fav) =>
            await Browser.OpenAsync(fav.Url, BrowserLaunchMode.SystemPreferred);

        private async Task OpenWebsiteAsync(FavoriteProject fav)
        {
            if (fav.PagesUrl != null)
                await Browser.OpenAsync(fav.PagesUrl, BrowserLaunchMode.SystemPreferred);
        }

        private async Task ShareAsync(FavoriteProject fav) =>
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = fav.Name,
                Text  = $"Check out {fav.Name}!\n{fav.Url}"
            });
    }
}