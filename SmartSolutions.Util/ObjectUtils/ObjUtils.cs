using System;

namespace SmartSolutions.Util.ObjectUtils
{
    public static class ObjUtils
    {
        /// <summary>
        /// Method which assign DbNull Value if null is assign
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object DbValueOrNull<T>(this T? value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }
    }
}
