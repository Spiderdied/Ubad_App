namespace Ubad.Helpers
{
    public static class ColorHelper
    {
        public static Color FromHex(string hex)
        {
            try   { return Color.FromArgb(hex); }
            catch { return Color.FromArgb("#6C63FF"); }
        }

        public static Color Darken(Color c, double amount = 0.15)
        {
            c.ToHsv(out float h, out float s, out float v);
            return Color.FromHsv(h, s, Math.Max(0f, v - (float)amount));
        }

        public static Color WithAlpha(Color c, double alpha) =>
            c.WithAlpha((float)alpha);

        public static string ToHex(Color c) =>
            $"#{(int)(c.Red * 255):X2}{(int)(c.Green * 255):X2}{(int)(c.Blue * 255):X2}";
    }
}