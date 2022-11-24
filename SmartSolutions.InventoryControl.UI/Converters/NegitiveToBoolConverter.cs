using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartSolutions.InventoryControl.UI.Converters
{
    /// <summary>
    /// Converter that will 
    /// </summary>
    public class NegitiveToBoolConverter : IValueConverter
    {
        /// <summary>
        /// If result is negitive return false else true
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>return  false if value is minus vice versa</returns>    
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retval = false;
            decimal val = 0;
            try
            {
                if (value != null)
                {
                    val = System.Convert.ToDecimal(value);
                }
                if (val < 0)
                {
                    retval = false;
                }
                else if (val >= 0)
                {
                    retval = true;
                }
            }
            catch (Exception)
            {

                throw;
            }           
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
