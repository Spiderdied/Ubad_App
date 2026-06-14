using Microsoft.Maui.Graphics;
using System;

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
            // 🛠️ حل بديل ومضمون 100%: تغميق اللون عن طريق تقليل الـ RGB
            float factor = (float)Math.Max(0.0, 1.0 - amount);
            return new Color(c.Red * factor, c.Green * factor, c.Blue * factor, c.Alpha);
        }

        public static Color WithAlpha(Color c, double alpha) =>
            c.WithAlpha((float)alpha);

        public static string ToHex(Color c) =>
            $"#{(int)(c.Red * 255):X2}{(int)(c.Green * 255):X2}{(int)(c.Blue * 255):X2}";
    }
}
