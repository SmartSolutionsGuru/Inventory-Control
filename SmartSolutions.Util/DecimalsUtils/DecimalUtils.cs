using System;

namespace SmartSolutions.Util.DecimalsUtils
{
    public static class DecimalUtils
    {
        #region PercentageOf calculations

        /// <summary>
        /// The numbers percentage
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>The result</returns>
        public static decimal PercentageOf(this decimal number, int percent)
        {
            return (decimal)(number * percent / 100);
        }

        /// <summary>
        /// Percentage of the number.
        /// </summary>
        /// <param name="percent">The percent</param>
        /// <param name="number">The Number</param>
        /// <returns>The result</returns>
        public static decimal PercentOf(this decimal position, int total)
        {
            decimal result = 0;
            if (position > 0 && total > 0)
            {
                result = (decimal)position / (decimal)total * 100;
            }
            return result;
        }

        /// <summary>
        /// The numbers percentage
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>The result</returns>
        public static decimal PercentageOf(this decimal number, decimal percent)
        {
            return (decimal)(number * percent / 100);
        }

        /// <summary>
        /// Percentage of the number.
        /// </summary>
        /// <param name="percent">The percent</param>
        /// <param name="number">The Number</param>
        /// <returns>The result</returns>
        public static decimal PercentOf(this decimal position, decimal total)
        {
            decimal result = 0;
            if (position > 0 && total > 0)
            {
                result = (decimal)position / (decimal)total * 100;
            }
            return result;
        }

        /// <summary>
        /// The numbers percentage
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>The result</returns>
        public static decimal PercentageOf(this decimal number, long percent)
        {
            return (decimal)(number * percent / 100);
        }

        /// <summary>
        /// Percentage of the number.
        /// </summary>
        /// <param name="percent">The percent</param>
        /// <param name="number">The Number</param>
        /// <returns>The result</returns>
        public static decimal PercentOf(this decimal position, long total)
        {
            decimal result = 0;
            if (position > 0 && total > 0)
            {
                result = (decimal)position / (decimal)total * 100;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Extension method to round a double value to a fixed number of significant figures instead of a fixed decimal places.
        /// </summary>
        /// <param name="d">Double we're rounding</param>
        /// <param name="digits">Number of significant figures</param>
        /// <returns>New double rounded to digits-significant figures</returns>
        public static decimal RoundToSignificantDigits(this decimal d, int digits)
        {
            if (d == 0) return 0;
            var scale = (decimal)Math.Pow(10, Math.Floor(Math.Log10((double)Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        /// <summary>
        /// Provides global smart rounding, numbers larger than 1000 will round to 4 decimal places,
        /// while numbers smaller will round to 7 significant digits
        /// </summary>
        public static decimal SmartRounding(this decimal input)
        {
            input = Normalize(input);

            // any larger numbers we still want some decimal places
            if (input > 1000)
            {
                return Math.Round(input, 4);
            }

            // this is good for forex and other small numbers
            //var d = (double)input;
            var d = (decimal)input;
            return (decimal)d.RoundToSignificantDigits(7);
        }

        public static decimal Normalize(this decimal input)
        {
            // http://stackoverflow.com/a/7983330/1582922
            return input / 1.000000000000000000000000000000000m;
        }

        //public static decimal Normalize(this decimal input)
        //{
        //    // http://stackoverflow.com/a/7983330/1582922
        //    return input / 1.000000000000000000000000000000000m;
        //}

        /// <summary>
        /// Extension method for faster string to decimal conversion. 
        /// </summary>
        /// <param name="str">String to be converted to positive decimal value</param>
        /// <remarks>
        /// Method makes some assuptions - always numbers, no "signs" +,- etc.
        /// Leading and trailing whitespace chars are ignored
        /// </remarks>
        /// <returns>Decimal value of the string</returns>
        /// 
        public static decimal ToDecimal(this string str)
        {
            decimal res = 0;
            decimal.TryParse(str.Replace('.', ','), out res);
            return res;
        }
        //public static decimal ToDecimal(this string str)
        //{
        //    long value = 0;
        //    var decimalPlaces = 0;
        //    var hasDecimals = false;
        //    var index = 0;
        //    var length = str.Length;

        //    while (index < length && char.IsWhiteSpace(str[index]))
        //    {
        //        index++;
        //    }

        //    while (index < length)
        //    {
        //        var ch = str[index++];
        //        if (ch == '.')
        //        {
        //            hasDecimals = true;
        //            decimalPlaces = 0;
        //        }
        //        else if (char.IsWhiteSpace(ch))
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            value = value * 10 + (ch - '0');
        //            decimalPlaces++;
        //        }
        //    }

        //    var lo = (int)value;
        //    var mid = (int)(value >> 32);
        //    return new decimal(lo, mid, 0, false, (byte)(hasDecimals ? decimalPlaces : 0));
        //}

        
    }
}