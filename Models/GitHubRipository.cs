namespace Ubad.Models
{
    public class GitHubRepository
    {
        public string  Id              { get; set; } = string.Empty;
        public string  Name            { get; set; } = string.Empty;
        public string  FullName        { get; set; } = string.Empty;
        public string  Description     { get; set; } = string.Empty;
        public string  Url             { get; set; } = string.Empty;
        public string  HomepageUrl     { get; set; } = string.Empty;
        public int     Stars           { get; set; }
        public int     Forks           { get; set; }
        public int     Watchers        { get; set; }
        public int     OpenIssues      { get; set; }
        public string  PrimaryLanguage { get; set; } = string.Empty;
        public string  LanguageColor   { get; set; } = "#6C63FF";
        public bool    IsPrivate       { get; set; }
        public bool    IsFork          { get; set; }
        public bool    HasPages        { get; set; }
        public string? GitHubPagesUrl  { get; set; }
        public DateTime? CreatedAt     { get; set; }
        public DateTime? UpdatedAt     { get; set; }
        public DateTime? PushedAt      { get; set; }
        public List<string> Topics     { get; set; } = new();

        // Runtime state
        public bool IsFavorite         { get; set; }
        public string DisplayImageUrl  { get; set; } = string.Empty;

        // Computed
        public string UpdatedAtDisplay =>
            UpdatedAt.HasValue
                ? Helpers.DateHelper.TimeAgo(UpdatedAt.Value)
                : "Unknown";

        public bool HasWebsite =>
            HasPages && !string.IsNullOrWhiteSpace(GitHubPagesUrl);

        public string StarsDisplay =>
            Stars >= 1000 ? $"{Stars / 1000.0:F1}k" : Stars.ToString();
    }
}