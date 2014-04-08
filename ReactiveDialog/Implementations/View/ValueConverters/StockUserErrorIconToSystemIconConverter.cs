using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace ReactiveDialog.Implementations.View.ValueConverters
{
    /// <summary>
    /// Converts a StockUserErrorIcon to the corresponding system icon.
    /// </summary>
    [ValueConversion(typeof (StockUserErrorIcon), typeof (BitmapSource))]
    public class StockUserErrorIconToSystemIconConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (!(value is StockUserErrorIcon))
            {
                return null;
            }

            var messageType = (StockUserErrorIcon)value;
            Icon icon;

            switch (messageType)
            {
                case StockUserErrorIcon.Critical:
                    icon = SystemIcons.Hand;
                    break;
                case StockUserErrorIcon.Error:
                    icon = SystemIcons.Error;
                    break;
                case StockUserErrorIcon.Question:
                    icon = SystemIcons.Question;
                    break;
                case StockUserErrorIcon.Warning:
                    icon = SystemIcons.Warning;
                    break;
                case StockUserErrorIcon.Notice:
                    icon = SystemIcons.Information;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                                                       Int32Rect.Empty,
                                                       BitmapSizeOptions.FromEmptyOptions());
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}