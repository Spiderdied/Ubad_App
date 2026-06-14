using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ubad.Configurations;
using Ubad.Models;

namespace Ubad.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient    _http;
        private readonly ICacheService _cache;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GitHubService(HttpClient http, ICacheService cache)
        {
            _http  = http;
            _cache = cache;
            ConfigureHttpClient();
        }

        // ── Setup ─────────────────────────────────────────────────

        private void ConfigureHttpClient()
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.UserAgent
                 .ParseAdd($"Ubad/{AppConfig.AppVersion}");
            _http.DefaultRequestHeaders.Accept
                 .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(AppConfig.GitHubToken))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppConfig.GitHubToken);
            }
        }

        // ── Profile ───────────────────────────────────────────────

        public async Task<GitHubProfile?> GetProfileAsync(CancellationToken ct = default)
        {
            const string cacheKey = "github_profile";

            var cached = await _cache.GetAsync<GitHubProfile>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var url      = $"{AppConfig.GitHubRestEndpoint}/users/{AppConfig.GitHubUsername}";
                var response = await _http.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json    = await response.Content.ReadAsStringAsync(ct);
                var rawUser = JsonDocument.Parse(json).RootElement;

                var profile = new GitHubProfile
                {
                    Login       = rawUser.TryGet("login"),
                    Name        = rawUser.TryGet("name"),
                    Bio         = rawUser.TryGet("bio"),
                    AvatarUrl   = rawUser.TryGet("avatar_url"),
                    Url         = rawUser.TryGet("html_url"),
                    Company     = rawUser.TryGet("company"),
                    Location    = rawUser.TryGet("location"),
                    Email       = rawUser.TryGet("email"),
                    WebsiteUrl  = rawUser.TryGet("blog"),
                    Followers   = rawUser.TryGetInt("followers"),
                    Following   = rawUser.TryGetInt("following"),
                    PublicRepos = rawUser.TryGetInt("public_repos"),
                };

                if (rawUser.TryGetProperty("created_at", out var ca))
                    profile.CreatedAt = ca.GetDateTime();

                await _cache.SetAsync(cacheKey, profile, AppConfig.CacheExpiryMinutes);
                return profile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GitHubService] GetProfile error: {ex.Message}");
                return null;
            }
        }

        // ── Pinned Repositories ───────────────────────────────────
        // ✅ GraphQL أولاً (يحتاج token) — Fallback لـ REST تلقائياً

        public async Task<List<GitHubRepository>> GetPinnedRepositoriesAsync(
            CancellationToken ct = default)
        {
            const string cacheKey = "pinned_repos";

            var cached = await _cache.GetAsync<List<GitHubRepository>>(cacheKey);
            if (cached != null) return cached;

            List<GitHubRepository> repos;

            // GraphQL — يعمل فقط مع token
            if (!string.IsNullOrWhiteSpace(AppConfig.GitHubToken))
            {
                repos = await TryGetPinnedViaGraphQlAsync(ct);
                if (repos.Count > 0)
                {
                    await _cache.SetAsync(cacheKey, repos, AppConfig.CacheExpiryMinutes);
                    return repos;
                }
            }

            // ✅ Fallback: REST API — أفضل 6 repos
            repos = await GetTopReposViaRestAsync(ct);

            if (repos.Count > 0)
                await _cache.SetAsync(cacheKey, repos, AppConfig.CacheExpiryMinutes);

            return repos;
        }

        // ── GraphQL Query ─────────────────────────────────────────

        private async Task<List<GitHubRepository>> TryGetPinnedViaGraphQlAsync(
            CancellationToken ct)
        {
            var query = $$"""
            {
              "query": "query { user(login: \"{{AppConfig.GitHubUsername}}\") { pinnedItems(first: 6, types: REPOSITORY) { nodes { ... on Repository { id name description url homepageUrl stargazerCount forkCount watchers { totalCount } openIssues: issues(states: OPEN) { totalCount } primaryLanguage { name color } isPrivate isFork createdAt updatedAt pushedAt repositoryTopics(first: 10) { nodes { topic { name } } } } } } } }"
            }
            """;

            try
            {
                var content  = new StringContent(query, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(
                    AppConfig.GitHubGraphQlEndpoint, content, ct);

                if (!response.IsSuccessStatusCode) return new();

                var json = await response.Content.ReadAsStringAsync(ct);
                var doc  = JsonDocument.Parse(json);

                // التحقق من غياب errors
                if (doc.RootElement.TryGetProperty("errors", out _)) return new();

                var pinnedNodes = doc.RootElement
                    .GetProperty("data")
                    .GetProperty("user")
                    .GetProperty("pinnedItems")
                    .GetProperty("nodes");

                var repos = new List<GitHubRepository>();

                foreach (var node in pinnedNodes.EnumerateArray())
                {
                    var repo = ParseRepositoryFromGraphQl(node);
                    repo.HasPages = await CheckGitHubPagesAsync(
                        AppConfig.GitHubUsername, repo.Name, ct);

                    if (repo.HasPages)
                        repo.GitHubPagesUrl =
                            $"{AppConfig.GitHubPagesBase}/{repo.Name}/";

                    repos.Add(repo);
                }

                return repos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GitHubService] GraphQL error: {ex.Message}");
                return new();
            }
        }

        // ── REST Fallback ─────────────────────────────────────────

        private async Task<List<GitHubRepository>> GetTopReposViaRestAsync(
            CancellationToken ct)
        {
            try
            {
                var url = $"{AppConfig.GitHubRestEndpoint}/users/{AppConfig.GitHubUsername}" +
                          "/repos?sort=pushed&per_page=6&type=public";

                var response = await _http.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json  = await response.Content.ReadAsStringAsync(ct);
                var array = JsonDocument.Parse(json).RootElement;
                var repos = new List<GitHubRepository>();

                foreach (var node in array.EnumerateArray())
                {
                    var repo = ParseRepositoryRest(node);
                    repo.HasPages = await CheckGitHubPagesAsync(
                        AppConfig.GitHubUsername, repo.Name, ct);

                    if (repo.HasPages)
                        repo.GitHubPagesUrl =
                            $"{AppConfig.GitHubPagesBase}/{repo.Name}/";

                    repos.Add(repo);
                }

                return repos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GitHubService] REST fallback error: {ex.Message}");
                return new();
            }
        }

        // ── Single Repo ───────────────────────────────────────────

        public async Task<GitHubRepository?> GetRepositoryAsync(
            string repoName, CancellationToken ct = default)
        {
            var cacheKey = $"repo_{repoName}";
            var cached   = await _cache.GetAsync<GitHubRepository>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var url = $"{AppConfig.GitHubRestEndpoint}/repos/" +
                          $"{AppConfig.GitHubUsername}/{repoName}";
                var response = await _http.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                var node = JsonDocument.Parse(json).RootElement;
                var repo = ParseRepositoryRest(node);

                await _cache.SetAsync(cacheKey, repo, AppConfig.CacheExpiryMinutes);
                return repo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GitHubService] GetRepository error: {ex.Message}");
                return null;
            }
        }

        // ── GitHub Pages Check ────────────────────────────────────

        public async Task<bool> CheckGitHubPagesAsync(
            string username, string repoName, CancellationToken ct = default)
        {
            var url = $"https://{username}.github.io/{repoName}/";
            try
            {
                var req  = new HttpRequestMessage(HttpMethod.Head, url);
                var resp = await _http.SendAsync(req, ct);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ── Parsers ───────────────────────────────────────────────

        private static GitHubRepository ParseRepositoryFromGraphQl(JsonElement node)
        {
            var repo = new GitHubRepository
            {
                Id          = node.TryGet("id"),
                Name        = node.TryGet("name"),
                FullName    = $"{AppConfig.GitHubUsername}/{node.TryGet("name")}",
                Description = node.TryGet("description"),
                Url         = node.TryGet("url"),
                HomepageUrl = node.TryGet("homepageUrl"),
                Stars       = node.TryGetInt("stargazerCount"),
                Forks       = node.TryGetInt("forkCount"),
                IsPrivate   = node.TryGetBool("isPrivate"),
                IsFork      = node.TryGetBool("isFork"),
            };

            if (node.TryGetProperty("watchers", out var w))
                repo.Watchers = w.TryGetInt("totalCount");

            if (node.TryGetProperty("openIssues", out var oi))
                repo.OpenIssues = oi.TryGetInt("totalCount");

            if (node.TryGetProperty("primaryLanguage", out var lang) &&
                lang.ValueKind != JsonValueKind.Null)
            {
                repo.PrimaryLanguage = lang.TryGet("name");
                repo.LanguageColor   = lang.TryGet("color");
                if (string.IsNullOrEmpty(repo.LanguageColor))
                    repo.LanguageColor = AppConfig.PrimaryColor;
            }

            if (node.TryGetProperty("createdAt", out var ca) &&
                DateTime.TryParse(ca.GetString(), out var caDt))
                repo.CreatedAt = caDt;

            if (node.TryGetProperty("updatedAt", out var ua) &&
                DateTime.TryParse(ua.GetString(), out var uaDt))
                repo.UpdatedAt = uaDt;

            if (node.TryGetProperty("pushedAt", out var pa) &&
                DateTime.TryParse(pa.GetString(), out var paDt))
                repo.PushedAt = paDt;

            if (node.TryGetProperty("repositoryTopics", out var rt) &&
                rt.TryGetProperty("nodes", out var topics))
            {
                foreach (var t in topics.EnumerateArray())
                    if (t.TryGetProperty("topic", out var tp))
                        repo.Topics.Add(tp.TryGet("name"));
            }

            return repo;
        }

        private static GitHubRepository ParseRepositoryRest(JsonElement node)
        {
            var repo = new GitHubRepository
            {
                Id          = node.TryGetInt("id").ToString(),
                Name        = node.TryGet("name"),
                FullName    = node.TryGet("full_name"),
                Description = node.TryGet("description"),
                Url         = node.TryGet("html_url"),
                HomepageUrl = node.TryGet("homepage"),
                Stars       = node.TryGetInt("stargazers_count"),
                Forks       = node.TryGetInt("forks_count"),
                Watchers    = node.TryGetInt("watchers_count"),
                OpenIssues  = node.TryGetInt("open_issues_count"),
                IsPrivate   = node.TryGetBool("private"),
                IsFork      = node.TryGetBool("fork"),
                HasPages    = node.TryGetBool("has_pages"),
            };

            if (node.TryGetProperty("language", out var lang) &&
                lang.ValueKind != JsonValueKind.Null)
                repo.PrimaryLanguage = lang.GetString() ?? string.Empty;

            repo.FullName = string.IsNullOrEmpty(repo.FullName)
                ? $"{AppConfig.GitHubUsername}/{repo.Name}"
                : repo.FullName;

            if (node.TryGetProperty("created_at", out var ca) &&
                DateTime.TryParse(ca.GetString(), out var caDt))
                repo.CreatedAt = caDt;

            if (node.TryGetProperty("updated_at", out var ua) &&
                DateTime.TryParse(ua.GetString(), out var uaDt))
                repo.UpdatedAt = uaDt;

            if (node.TryGetProperty("pushed_at", out var pa) &&
                DateTime.TryParse(pa.GetString(), out var paDt))
                repo.PushedAt = paDt;

            return repo;
        }
    }

    // ── JSON Extensions ───────────────────────────────────────────

    internal static class JsonElementExtensions
    {
        public static string TryGet(this JsonElement el, string prop)
        {
            if (el.TryGetProperty(prop, out var v) &&
                v.ValueKind != JsonValueKind.Null)
                return v.GetString() ?? string.Empty;
            return string.Empty;
        }

        public static int TryGetInt(this JsonElement el, string prop)
        {
            if (el.TryGetProperty(prop, out var v) &&
                v.ValueKind == JsonValueKind.Number)
                return v.GetInt32();
            return 0;
        }

        public static bool TryGetBool(this JsonElement el, string prop)
        {
            if (el.TryGetProperty(prop, out var v) &&
                (v.ValueKind == JsonValueKind.True ||
                 v.ValueKind == JsonValueKind.False))
                return v.GetBoolean();
            return false;
        }

        public static int TryGetInt(this JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Number) return el.GetInt32();
            return 0;
        }

        public static int TryGetInt(this JsonElement el, string prop, int fallback)
        {
            if (el.TryGetProperty(prop, out var v) &&
                v.ValueKind == JsonValueKind.Number)
                return v.GetInt32();
            return fallback;
        }
    }
}
