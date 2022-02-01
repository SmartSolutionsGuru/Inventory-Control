using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartSolutions.Util.StrUtils
{
    public static class StringUtils
    {
        /// <summary>
        /// Method which assign DbNull Value if null is assign
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static object DbValueOrNull<T>(this T value) where T : struct
        //{
        //    //return string.IsNullOrEmpty(value) ? value : DBNull.Value;
        //    return value.HasValue ? (object)value.Value : DBNull.Value;
        //}


        /// <summary>
        /// Method For Capitalizing First Letter Of Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(this String input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
    }
}
