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
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict == null || dict.ContainsKey(key))
            {
                return false;
            }

            dict.Add(key, value);

            return true;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }

            if (dict.ContainsKey(key))
            {
                dict[key] = updateValueFactory(key, addValue);
            }
            else
            {
                dict[key] = addValue;
            }

            return dict[key];
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> @thisDict, Dictionary<TKey, TValue> @thatDict)
        {
            if (@thisDict == null)
            {
                throw new ArgumentNullException("thisDict");
            }

            if (@thatDict == null)
            {
                throw new ArgumentNullException("thatDict");
            }

            var result = new Dictionary<TKey, TValue>(@thisDict.Count + @thatDict.Count);

            foreach (var pair in @thisDict)
            {
                result.Add(pair.Key, pair.Value);
            }

            foreach (var pair in @thatDict)
            {
                TryAdd(result, pair.Key, pair.Value);
            }

            return result;
        }
    }
}
