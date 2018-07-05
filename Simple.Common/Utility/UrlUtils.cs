using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Utility
{
    public class UrlUtils
    {
        private static readonly char[] TrimChars = { '/', '\\' };
        private const char UrlSeparatorChar = '/';

        public static string Combine(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                return relativeUrl;
            }

            if (string.IsNullOrEmpty(relativeUrl))
            {
                return baseUrl;
            }

            return string.Format("{0}{2}{1}", baseUrl.TrimEnd(TrimChars), relativeUrl.TrimStart(TrimChars), UrlSeparatorChar);
        }

        public static string Combine(params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException("urls");
            }

            var finalUrl = new StringBuilder(urls.Length);

            for (int i = 0; i < urls.Length; i++)
            {
                if (finalUrl.Length == 0)
                {
                    finalUrl.Append((urls[i] ?? string.Empty).Trim(TrimChars));
                }
                else
                {
                    char ch = finalUrl[finalUrl.Length - 1];
                    if (ch != UrlSeparatorChar)
                    {
                        finalUrl.Append(UrlSeparatorChar);
                    }

                    finalUrl.Append((urls[i] ?? string.Empty).Trim(TrimChars));
                }
            }

            return finalUrl.ToString();
        }

        public static string DecodeParameter(string value)
        {
            value = value.Replace("-", "+").Replace("_", "/").Replace("$", "=");
            byte[] arrBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(arrBytes);
        }

        public static string EncodeParameter(string value)
        {
            byte[] arrBytes = Encoding.UTF8.GetBytes(value);
            value = Convert.ToBase64String(arrBytes);
            value = value.Replace("+", "-").Replace("/", "_").Replace("=", "$");
            return value;
        }

        public static string GetParameterName(string pair)
        {
            string[] nameValues = pair.Split('=');
            return nameValues[0];
        }

        public static string GetParameterValue(string pair)
        {
            string[] nameValues = pair.Split('=');
            if (nameValues.Length > 1)
            {
                return nameValues[1];
            }
            return "";
        }
    }
}
