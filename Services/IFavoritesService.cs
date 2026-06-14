using Ubad.Models;

namespace Ubad.Services
{
    public interface IFavoritesService
    {
        Task<List<FavoriteProject>> GetAllAsync();
        Task<bool>                  IsFavoriteAsync(string repositoryId);
        Task                        AddAsync(FavoriteProject project);
        Task                        RemoveAsync(string repositoryId);
        Task                        ToggleAsync(GitHubRepository repo);
    }
}