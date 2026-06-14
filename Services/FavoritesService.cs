using System.Text.Json;
using Ubad.Models;

namespace Ubad.Services
{
    public class FavoritesService : IFavoritesService
    {
        private const string StorageKey = "ubad_favorites";

        private static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private async Task<List<FavoriteProject>> LoadAsync()
        {
            await Task.Yield();
            var raw = Preferences.Get(StorageKey, string.Empty);
            if (string.IsNullOrEmpty(raw)) return new();
            try   { return JsonSerializer.Deserialize<List<FavoriteProject>>(raw, _opts) ?? new(); }
            catch { return new(); }
        }

        private async Task SaveAsync(List<FavoriteProject> list)
        {
            await Task.Yield();
            Preferences.Set(StorageKey, JsonSerializer.Serialize(list, _opts));
        }

        public async Task<List<FavoriteProject>> GetAllAsync() => await LoadAsync();

        public async Task<bool> IsFavoriteAsync(string repositoryId)
        {
            var list = await LoadAsync();
            return list.Any(f => f.RepositoryId == repositoryId);
        }

        public async Task AddAsync(FavoriteProject project)
        {
            var list = await LoadAsync();
            if (list.All(f => f.RepositoryId != project.RepositoryId))
            {
                list.Add(project);
                await SaveAsync(list);
            }
        }

        public async Task RemoveAsync(string repositoryId)
        {
            var list = await LoadAsync();
            list.RemoveAll(f => f.RepositoryId == repositoryId);
            await SaveAsync(list);
        }

        public async Task ToggleAsync(GitHubRepository repo)
        {
            if (await IsFavoriteAsync(repo.Id))
            {
                await RemoveAsync(repo.Id);
                repo.IsFavorite = false;
            }
            else
            {
                await AddAsync(new FavoriteProject
                {
                    RepositoryId  = repo.Id,
                    Name          = repo.Name,
                    Description   = repo.Description,
                    Language      = repo.PrimaryLanguage,
                    LanguageColor = repo.LanguageColor,
                    Stars         = repo.Stars,
                    Url           = repo.Url,
                    PagesUrl      = repo.GitHubPagesUrl,
                });
                repo.IsFavorite = true;
            }
        }
    }
}