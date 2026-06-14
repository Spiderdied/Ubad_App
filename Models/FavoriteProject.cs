namespace Ubad.Models
{
    public class FavoriteProject
    {
        public string RepositoryId   { get; set; } = string.Empty;
        public string Name           { get; set; } = string.Empty;
        public string Description    { get; set; } = string.Empty;
        public string Language       { get; set; } = string.Empty;
        public string LanguageColor  { get; set; } = "#6C63FF";
        public int    Stars          { get; set; }
        public string Url            { get; set; } = string.Empty;
        public string? PagesUrl      { get; set; }
        public DateTime SavedAt      { get; set; } = DateTime.UtcNow;
    }
}