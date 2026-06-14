namespace Ubad.Configurations
{
    public static class AppConfig
    {
        // ── Identity ──────────────────────────────────────────────
        public const string AppName        = "Ubad";
        public const string AppVersion     = "1.0.0";
        public const string AppDescription = "Premium Portfolio Platform";

        // ── GitHub ────────────────────────────────────────────────
        public const string GitHubUsername   = "Spiderdied";
        public const string GitHubProfileUrl = "https://github.com/Spiderdied";
        public const string GitHubPagesBase  = "https://spiderdied.github.io";

        // ── GitHub GraphQL ────────────────────────────────────────
        public const string GitHubGraphQlEndpoint = "https://api.github.com/graphql";
        public const string GitHubRestEndpoint     = "https://api.github.com";

        // ── Token (optional – leave empty for public profile) ─────
        // If the profile is public, pinned repos work without a token.
        // Set via environment variable or app secrets for production.
        public static string GitHubToken =>
            Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? string.Empty;

        // ── Colors ────────────────────────────────────────────────
        public const string PrimaryColor   = "#6C63FF";
        public const string SecondaryColor = "#FF6584";
        public const string AccentColor    = "#43E97B";
        public const string SurfaceColor   = "#1E1E2E";
        public const string BackgroundDark = "#12121F";
        public const string CardDark       = "#1E1E2E";
        public const string CardLight      = "#FFFFFF";

        // ── Cache ─────────────────────────────────────────────────
        public const int CacheExpiryMinutes = 30;
        public const string CachePrefix     = "ubad_cache_";

        // ── Animation ─────────────────────────────────────────────
        public const uint AnimationDurationShort  = 200;
        public const uint AnimationDurationMedium = 400;
        public const uint AnimationDurationLong   = 600;

        // ── UI ────────────────────────────────────────────────────
        public const int CardCornerRadius  = 16;
        public const int PagePadding       = 20;
        public const int CardSpacing       = 16;
    }
}