using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApplication3
{
    public class DoubleScaler : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numberToScale = System.Convert.ToDouble(value);
            var scale = System.Convert.ToDouble(parameter);

            return numberToScale * scale;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}