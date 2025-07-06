using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.Frontend.Utils
{
    public class MetersToKilometersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int meters)
                return $"{meters / 1000.0:F2} km";
            if (value is double dmeters)
                return $"{dmeters / 1000.0:F2} km";
            return "0.00 km";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s.EndsWith("km") && double.TryParse(s.Replace("km", "").Trim(), out var km))
                return km * 1000;
            return 0;
        }
    }
}