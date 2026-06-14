using System.Globalization;

namespace Ubad.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture) =>
            value is bool b && b;

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            value is bool b && b;
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture) =>
            value is bool b && !b;

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            value is bool b && !b;
    }

    public class BoolToColorConverter : IValueConverter
    {
        public Color TrueColor  { get; set; } = Color.FromArgb("#6C63FF");
        public Color FalseColor { get; set; } = Color.FromArgb("#9090A8");

        public object Convert(object? value, Type targetType,
                              object? parameter, CultureInfo culture) =>
            value is bool b && b ? TrueColor : FalseColor;

        public object ConvertBack(object? value, Type targetType,
                                   object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}