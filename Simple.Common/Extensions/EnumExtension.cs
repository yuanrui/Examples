using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class EnumExtension
    {
        public static bool HasFlag(this Enum enumRef, Enum flag)
        {
            long value = Convert.ToInt64(enumRef);
            long flagVal = Convert.ToInt64(flag);

            return (value & flagVal) == flagVal;
        }

        public static string GetDescription(this Enum obj)
        {
            return GetDescription(obj, true);
        }

        public static string GetDescription(this Enum obj, bool isAppendDefault)
        {
            List<string> list = obj.ToString().Replace("\u0020", string.Empty).Split('\u002c').ToList();
            Type type = obj.GetType();
            string result = string.Empty;

            foreach (var item in list)
            {
                string name = Enum.GetName(type, Enum.Parse(type, item));

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                object[] atts = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (atts != null && atts.Length > 0)
                {
                    var description = ((DescriptionAttribute)atts[0]).Description;

                    if (!string.IsNullOrEmpty(description))
                    {
                        result = result + ((DescriptionAttribute)atts[0]).Description + "\u3001";
                        continue;
                    }
                }

                if (isAppendDefault)
                {
                    result = result + name + "\u3001";
                }
            }

            return result.Trim('\u3001');
        }

        public static List<string> GetDescriptions(this Enum obj)
        {
            return GetDescriptions(obj, true);
        }

        public static List<string> GetDescriptions(this Enum obj, bool isAppendDefault)
        {
            var source = obj.ToString().Replace("\u0020", string.Empty).Split('\u002c').ToList();
            var type = obj.GetType();
            var result = new List<string>();

            foreach (var item in source)
            {
                string name = Enum.GetName(type, Enum.Parse(type, item));

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                object[] atts = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (atts != null && atts.Length > 0)
                {
                    var description = ((DescriptionAttribute)atts[0]).Description;

                    if (!string.IsNullOrEmpty(description))
                    {
                        result.Add(description);
                        continue;
                    }
                }

                if (isAppendDefault)
                {
                    result.Add(name);
                }
            }

            return result;
        }

        public static string GetName(this Enum obj)
        {
            return Enum.GetName(obj.GetType(), obj);
        }

        public static Int32 ToInt32(this Enum obj)
        {
            return Convert.ToInt32(obj);
        }

        public static Byte ToByte(this Enum obj)
        {
            return Convert.ToByte(obj);
        }

        public static string[] ToStringArray(this Enum obj)
        {
            var result = obj.ToString().Replace("\u0020", string.Empty).Split('\u002c');
            return result;
        }

        public static int[] ToIntArray(this Enum obj)
        {
            var type = obj.GetType();
            var list = ToStringArray(obj);
            var result = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                result[i] = (int)Enum.Parse(type, list[i]);
            }

            return result;
        }

        public static Tuple<string, int>[] ToTuples(this Enum obj)
        {

            var type = obj.GetType();
            var list = ToStringArray(obj);
            var result = new Tuple<string, int>[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = Tuple.Create(list[i], (int)Enum.Parse(type, list[i]));
            }

            return result;
        }

        public static KeyValuePair<string, int>[] ToKeyValues(this Enum obj)
        {
            var type = obj.GetType();
            var list = ToStringArray(obj);
            var result = new KeyValuePair<string, int>[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                result[i] = new KeyValuePair<string, int>(list[i], (int)Enum.Parse(type, list[i]));
            }

            return result;
        }
    }
}
