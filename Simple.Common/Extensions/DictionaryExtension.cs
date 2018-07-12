using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// try add key value for dictionary, if key not exists then add key value, else do nothing.
        /// Thread unsafe
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic == null || dic.ContainsKey(key))
            {
                return false;
            }

            dic.Add(key, value);

            return true;
        }
    }
}
