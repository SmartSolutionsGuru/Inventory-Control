using SmartSolutions.InventoryControl.UI.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    public class BytesToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapSource) return value;
            byte[] val = value as byte[];
            return ImageHelper.ByteArrayToBitmapImage(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage val = value as BitmapImage;
            return val?.ToByteArray();
        }
    }
}
