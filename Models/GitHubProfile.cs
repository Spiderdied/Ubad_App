namespace Ubad.Models
{
    public class GitHubProfile
    {
        public string Login           { get; set; } = string.Empty;
        public string Name            { get; set; } = string.Empty;
        public string Bio             { get; set; } = string.Empty;
        public string AvatarUrl       { get; set; } = string.Empty;
        public string Url             { get; set; } = string.Empty;
        public string Company         { get; set; } = string.Empty;
        public string Location        { get; set; } = string.Empty;
        public string Email           { get; set; } = string.Empty;
        public string WebsiteUrl      { get; set; } = string.Empty;
        public int    Followers        { get; set; }
        public int    Following        { get; set; }
        public int    PublicRepos      { get; set; }
        public int    PinnedReposCount { get; set; }
        public DateTime? CreatedAt    { get; set; }
    }
}