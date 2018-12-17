using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Generic
{
    public class ParamDictionary : IDictionary<String, String>
    {
        static readonly Char[] DefaultParamSeparators = new[] { '&', ';' };
        static readonly Char[] ParamKeyValueSeparator = new[] { '=' };
        static readonly Char[] LeadingWhitespaceChars = new[] { ' ' };

        public static IEnumerable<KeyValuePair<String, String>> ParseToEnumerable(String queryString, params Char[] delimiters)
        {
            var items = (queryString ?? String.Empty).Split(delimiters ?? DefaultParamSeparators, StringSplitOptions.RemoveEmptyEntries);
            var rawPairs = items.Select(item => item.Split(ParamKeyValueSeparator, 2, StringSplitOptions.None));
            var pairs = rawPairs.Select(pair => new KeyValuePair<String, String>(
                Uri.UnescapeDataString(pair[0]).Replace('+', ' ').TrimStart(LeadingWhitespaceChars),
                pair.Length < 2 ? String.Empty : Uri.UnescapeDataString(pair[1]).Replace('+', ' ')));
            return pairs;
        }

        public static IDictionary<String, String> Parse(String queryString, params Char[] delimiters)
        {
            var d = ParseToEnumerable(queryString, delimiters)
                .GroupBy(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => String.Join(",", g.ToArray()), StringComparer.OrdinalIgnoreCase);

            return new ParamDictionary(d);
        }

        readonly IDictionary<String, String> _impl;



        ParamDictionary(IDictionary<String, String> impl)
        {
            _impl = impl;
        }

        IEnumerator<KeyValuePair<String, String>> IEnumerable<KeyValuePair<String, String>>.GetEnumerator()
        {
            return _impl.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _impl.GetEnumerator();
        }

        void ICollection<KeyValuePair<String, String>>.Add(KeyValuePair<String, String> item)
        {
            _impl.Add(item);
        }

        void ICollection<KeyValuePair<String, String>>.Clear()
        {
            _impl.Clear();
        }

        bool ICollection<KeyValuePair<String, String>>.Contains(KeyValuePair<String, String> item)
        {
            return _impl.Contains(item);
        }

        void ICollection<KeyValuePair<String, String>>.CopyTo(KeyValuePair<String, String>[] array, int arrayIndex)
        {
            _impl.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<String, String>>.Remove(KeyValuePair<String, String> item)
        {
            return _impl.Remove(item);
        }

        int ICollection<KeyValuePair<String, String>>.Count
        {
            get { return _impl.Count; }
        }

        bool ICollection<KeyValuePair<String, String>>.IsReadOnly
        {
            get { return _impl.IsReadOnly; }
        }

        bool IDictionary<String, String>.ContainsKey(String key)
        {
            return _impl.ContainsKey(key);
        }

        void IDictionary<String, String>.Add(String key, String value)
        {
            _impl.Add(key, value);
        }

        bool IDictionary<String, String>.Remove(String key)
        {
            return _impl.Remove(key);
        }

        bool IDictionary<String, String>.TryGetValue(String key, out String value)
        {
            return _impl.TryGetValue(key, out value);
        }

        String IDictionary<String, String>.this[String key]
        {
            get
            {
                String value;
                return _impl.TryGetValue(key, out value) ? value : default(String);
            }
            set { _impl[key] = value; }
        }

        ICollection<String> IDictionary<String, String>.Keys
        {
            get { return _impl.Keys; }
        }

        ICollection<String> IDictionary<String, String>.Values
        {
            get { return _impl.Values; }
        }
    }
}