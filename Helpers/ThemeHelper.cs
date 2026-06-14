namespace Ubad.Helpers
{
    public static class ThemeHelper
    {
        public static bool IsDark =>
            Application.Current?.RequestedTheme == AppTheme.Dark;

        public static Color Surface =>
            IsDark ? Color.FromArgb("#1E1E2E") : Color.FromArgb("#F8F8FF");

        public static Color Background =>
            IsDark ? Color.FromArgb("#12121F") : Color.FromArgb("#F0F0FA");

        public static Color CardBackground =>
            IsDark ? Color.FromArgb("#252535") : Color.FromArgb("#FFFFFF");

        public static Color TextPrimary =>
            IsDark ? Color.FromArgb("#E8E8F0") : Color.FromArgb("#1A1A2E");

        public static Color TextSecondary =>
            IsDark ? Color.FromArgb("#9090A8") : Color.FromArgb("#5A5A7A");

        public static Color Primary =>
            Color.FromArgb("#6C63FF");

        public static Color Accent =>
            Color.FromArgb("#43E97B");
    }
}