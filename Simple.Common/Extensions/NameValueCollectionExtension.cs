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
    }
}
