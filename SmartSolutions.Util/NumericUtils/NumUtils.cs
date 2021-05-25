using System;

namespace SmartSolutions.Util.NumericUtils
{
    public static class NumUtils
    {

		public static int ToInt(this string stringText,int defaultValue = 0)
		{
			try
			{
				if (!string.IsNullOrEmpty(stringText))
				{
					int resultValue;
					if (int.TryParse(stringText, out resultValue)) return resultValue;
					return defaultValue;
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			return defaultValue;
		}
        public static int? ToNullableInt(this string stringText,int? defaultValue = null)
        {
			try
			{
				if(!string.IsNullOrEmpty(stringText))
				{
					int resultValue;
					if (int.TryParse(stringText, out resultValue)) return resultValue;
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
