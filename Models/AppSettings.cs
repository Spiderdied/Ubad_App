namespace Ubad.Models
{
    public class AppSettings
    {
        public string ThemeMode            { get; set; } = "System"; // Light | Dark | System
        public bool   EnableAnimations     { get; set; } = true;
        public bool   EnableCache          { get; set; } = true;
        public int    CacheExpiryMinutes   { get; set; } = 30;
        public string SortBy              { get; set; } = "Stars";   // Stars | Updated | Name
        public bool   ShowPrivateRepos     { get; set; } = false;
    }
}