using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        private readonly BrushConverter converter = new BrushConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush) return value;
            Brush retVal = null;
            try
            {
                if (value != null)
                    retVal = converter.ConvertFrom(value) as Brush;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = null;
            try
            {
                if (value is Brush && value != null)
                    retVal = converter.ConvertToString(value);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retVal;
        }
    }
}
