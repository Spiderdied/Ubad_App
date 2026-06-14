using System.Globalization;

namespace Ubad.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture) =>
            value != null && !string.IsNullOrWhiteSpace(value.ToString());

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class EmptyListToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
        {
            if (value is System.Collections.ICollection col)
                return col.Count > 0;
            return false;
        }

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class StarCountConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
        {
            if (value is int stars)
                return stars >= 1000 ? $"{stars / 1000.0:F1}k" : stars.ToString();
            return "0";
        }

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
        {
            try
            {
                if (value is string hex && !string.IsNullOrEmpty(hex))
                    return Color.FromArgb(hex.StartsWith('#') ? hex : "#" + hex);
            }
            catch { }
            return Color.FromArgb("#6C63FF");
        }

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture) =>
            value is int i && i > 0;

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}