using Ubad.Models;

namespace Ubad.Services
{
    public interface IGitHubService
    {
        Task<GitHubProfile?>            GetProfileAsync(CancellationToken ct = default);
        Task<List<GitHubRepository>>    GetPinnedRepositoriesAsync(CancellationToken ct = default);
        Task<GitHubRepository?>         GetRepositoryAsync(string repoName, CancellationToken ct = default);
        Task<bool>                       CheckGitHubPagesAsync(string username, string repoName, CancellationToken ct = default);
    }
}