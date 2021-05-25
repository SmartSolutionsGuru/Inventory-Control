using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.Util.BooleanUtils
{
    public static class BoolUtils
    {

        public static bool ToBoolean(this string stringText, bool defaultValue = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(null))
                {
                    bool resultValue;
                    if (bool.TryParse(stringText, out resultValue)) return resultValue;
                    else if ("1".Equals(stringText) || "YES".Equals(stringText?.ToUpper())) return true;
                    else if ("0".Equals(stringText) || "NO".Equals(stringText?.ToUpper())) return false;
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return defaultValue;
        }
        public static bool? ToNullableBoolean(this string stringText, bool? defaultValue = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(stringText))
                {
                    bool resultValue;
                    if (bool.TryParse(stringText, out resultValue)) return resultValue;
                    else if ("1".Equals(stringText) || "YES".Equals(stringText?.ToUpper())) return true;
                    else if ("0".Equals(stringText) || "NO".Equals(stringText?.ToUpper())) return false;
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
