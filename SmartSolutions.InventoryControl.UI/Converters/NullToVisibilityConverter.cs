using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value != null;
            string param = "";

            if (parameter != null)
                param = parameter.ToString();

            if (param.ToLower().Contains("inverse"))
                val = !val;

            Visibility hiddenVisibility = Visibility.Collapsed;
            if (param.ToLower().Contains("hidden"))
                hiddenVisibility = Visibility.Hidden;

            Visibility retVal;
            if (val)
                retVal = Visibility.Visible;
            else
                retVal = hiddenVisibility;

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility val = (Visibility)value;
            string param = "";

            if (parameter != null)
                param = parameter.ToString();

            bool retVal;
            if (val == Visibility.Visible)
                retVal = true;
            else
                retVal = false;

            if (param.ToLower().Equals("inverse"))
                retVal = !retVal;
            return retVal;
        }
    }
}
