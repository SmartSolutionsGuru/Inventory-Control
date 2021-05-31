using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.Util.DateAndTimeUtils
{
    public static class DateTimeUtils
    {
        public static DateTime? ToNullableDateTime(this  string stringText, DateTime? defaultValue = null, string format = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(stringText))
                {
                    DateTime resultValue;
                    if (format != null)
                    {
                        if (DateTime.TryParse(stringText, out resultValue)) return resultValue;
                    }
                    else
                    {
                        if (DateTime.TryParseExact(stringText,format, null, System.Globalization.DateTimeStyles.None, out resultValue)) return resultValue;
                    }
                    return defaultValue;

                }
            }
            catch (Exception ex)
            {

                ex.ToString();
            }
            return defaultValue;
        }
    }
}
