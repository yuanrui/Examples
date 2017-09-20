using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Simple.Common.Utility
{
    public class EnumUtils
    {
        public static Dictionary<string, int> AsDictionary<T>()
        {
            return AsDictionary<T>(false);
        }

        public static Dictionary<string, int> AsDictionary<T>(bool isGetDescription)
        {
            var type = typeof (T);
            
            var result = new Dictionary<string, int>();

            foreach (int item in Enum.GetValues(type))
            {
                if (isGetDescription)
                {
                    var atts = type.GetField(Enum.GetName(type, item)).GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (atts != null && atts.Any())
                    {
                        var description = ((DescriptionAttribute)atts[0]).Description;
                        if (!string.IsNullOrEmpty(description))
                        {
                            result.Add(description, item);
                            continue;
                        }
                    }
                }
                
                result.Add(Enum.GetName(type, item), item);
            }

            return result;
        }

        public static KeyValuePair<string, int>[] AsKeyValues<T>()
        {
            return AsKeyValues<T>(false);
        }

        public static KeyValuePair<string, int>[] AsKeyValues<T>(bool isGetDescription)
        {
            return AsKeyValues<T>(isGetDescription, null);
        }

        public static KeyValuePair<string, int>[] AsKeyValues<T>(bool isGetDescription, params string[] removedItems)
        {
            var type = typeof(T);
            var values = Enum.GetValues(type);
            var result = new List<KeyValuePair<string, int>>(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                var item = (int)values.GetValue(i);
                
                if (removedItems != null)
                {
                    var txt = Enum.GetName(type, item);
                    if (removedItems.Any(m => m == txt))
                    {
                        continue;
                    }
                }

                if (isGetDescription)
                {
                    var atts = type.GetField(Enum.GetName(type, item)).GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (atts != null && atts.Any())
                    {
                        var description = ((DescriptionAttribute)atts[0]).Description;
                        if (!string.IsNullOrEmpty(description))
                        {
                            result.Add(new KeyValuePair<string, int>(description, item));
                            continue;
                        }
                    }
                }

                result.Add(new KeyValuePair<string, int>(Enum.GetName(type, item), item));
            }

            return result.ToArray();
        }

        public static string[] GetNames<T>()
        {
            var type = typeof (T);
            
            return Enum.GetNames(type);
        }

        public static int[] GetValues<T>()
        {
            var type = typeof(T);
            var valueArray = Enum.GetValues(type);
            var result = new int[valueArray.Length];

            for (int i = 0; i < valueArray.Length; i++)
            {
                result[i] = (int)valueArray.GetValue(i);
            }

            return result;
        }
    }
}
