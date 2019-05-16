using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class NameValueCollectionExtension
    {
        public static Dictionary<String, String> ToDictionary(this NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return new Dictionary<String, String>(0);
            }

            var result = new Dictionary<String, String>(collection.Count);

            foreach (String key in collection.Keys)
            {
                if (result.ContainsKey(key))
                {
                    continue;
                }

                result.Add(key, collection[key]);
            }

            return result;
        }

        public static KeyValuePair<String, String>[] ToKeyValues(this NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return new KeyValuePair<String, String>[0];
            }

            var result = new KeyValuePair<String, String>[collection.Count];

            for (int i = 0; i < collection.Count; i++)
            {
                result[i] = new KeyValuePair<String, String>(collection.Keys[i], collection[i]);
            }

            return result;
        }

        internal static T GetValue<T>(NameValueCollection collection, string key, Func<string, T> parseValue) where T : struct
        {
            string value = collection[key];

            if (!string.IsNullOrWhiteSpace(value))
            {
                return parseValue(value);
            }

            return default(T);
        }

        internal static T GetValue<T>(NameValueCollection collection, string key, Func<string, T> parseValue, T @defaultValue) where T : struct
        {
            string value = collection[key].IsNullOrEmptyReturn(defaultValue.ToString());

            try
            {
                return parseValue(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static Byte GetByte(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Byte.Parse);
        }

        public static Byte GetByte(this NameValueCollection collection, string key, Byte @defaultValue)
        {
            return GetValue(collection, key, Byte.Parse, defaultValue);
        }

        public static Int16 GetInt16(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Int16.Parse);
        }

        public static Int16 GetInt16(this NameValueCollection collection, string key, Int16 @defaultValue)
        {
            return GetValue(collection, key, Int16.Parse, defaultValue);
        }

        public static Int32 GetInt32(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Int32.Parse);
        }

        public static Int32 GetInt32(this NameValueCollection collection, string key, Int32 @defaultValue)
        {
            return GetValue(collection, key, Int32.Parse, defaultValue);
        }

        public static Int64 GetInt64(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Int64.Parse);
        }

        public static Int64 GetInt64(this NameValueCollection collection, string key, Int64 @defaultValue)
        {
            return GetValue(collection, key, Int64.Parse, defaultValue);
        }

        public static Decimal GetDecimal(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Decimal.Parse);
        }
        public static Decimal GetDecimal(this NameValueCollection collection, string key, Decimal @defaultValue)
        {
            return GetValue(collection, key, Decimal.Parse, defaultValue);
        }

        public static Double GetDouble(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Double.Parse);
        }

        public static Double GetDouble(this NameValueCollection collection, string key, Double @defaultValue)
        {
            return GetValue(collection, key, Double.Parse, defaultValue);
        }

        public static Single GetSingle(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Single.Parse);
        }

        public static Single GetSingle(this NameValueCollection collection, string key, Single @defaultValue)
        {
            return GetValue(collection, key, Single.Parse, defaultValue);
        }

        public static Boolean GetBoolean(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, Boolean.Parse);
        }

        public static Boolean GetBoolean(this NameValueCollection collection, string key, Boolean @defaultValue)
        {
            return GetValue(collection, key, Boolean.Parse, defaultValue);
        }

        public static DateTime GetDateTime(this NameValueCollection collection, string key)
        {
            return GetValue(collection, key, DateTime.Parse);
        }

        public static DateTime GetDateTime(this NameValueCollection collection, string key, DateTime @defaultValue)
        {
            return GetValue(collection, key, DateTime.Parse, defaultValue);
        }

        public static String GetString(this NameValueCollection collection, string key, String @defaultValue)
        {
            if (collection == null)
            {
                return defaultValue;
            }

            var value = collection[key];
            if (String.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }

        public static void CopyTo(this NameValueCollection fromCollection, NameValueCollection toCollection)
        {
            if (fromCollection == null || fromCollection.Count == 0)
            {
                return;
            }

            for (int i = 0; i < fromCollection.Count; i++)
            {
                var key = fromCollection.GetKey(i);
                toCollection[key] = fromCollection[i];
            }
        }
    }
}
