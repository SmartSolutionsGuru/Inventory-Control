using System;
using System.Collections.Generic;

namespace SmartSolutions.Util.DictionaryUtils
{
    public static  class DictUtil
    {
        /// <summary>
        /// Utility Method that will Return 
        /// the value of Dictonary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name=""></param>
        /// <param name="dictonary"></param>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static V GetValueFromDictonary<T,V>(this IDictionary<T,V>dictonary,T Key,V defaultValue = default(V))
        {
            V resultValue = defaultValue;
            try
            {
                if (dictonary?.ContainsKey(Key) ?? false)
                    resultValue = dictonary[Key];
            }
            catch (Exception ex)
            {

                ex.ToString();
            }
            return resultValue;
        }
    }
}
