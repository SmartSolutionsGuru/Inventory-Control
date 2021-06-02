using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    public class CalendarDayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var daynames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
            string dayname = value.ToString();
            return daynames.First(t => t.StartsWith(dayname)).Substring(0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
