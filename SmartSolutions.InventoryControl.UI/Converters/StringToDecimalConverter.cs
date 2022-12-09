using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            decimal? convertDecimal = null;
            if (!string.IsNullOrEmpty(val))
            {
                 convertDecimal = Decimal.Parse((string)value, NumberStyles.AllowThousands
                       | NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol);
            }
            else
            {
                convertDecimal = value as decimal?;
                if (convertDecimal != null)
                    convertDecimal = value as decimal?;
            }
          
            return convertDecimal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
            var  val = value as string;
            decimal? convertDecimal = null;
            if (!string.IsNullOrEmpty(val))
            {
                convertDecimal = Decimal.Parse((string)value, NumberStyles.AllowThousands
                      | NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol);
            }
            return convertDecimal;
        }
    }
}
