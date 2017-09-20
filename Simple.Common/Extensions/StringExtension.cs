using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// remove input string blank space \t \n \r 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Clean(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return input.Replace("\u3000", string.Empty).Replace("\u0020", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
        }
        
        /// <summary>
        /// trim input string blank space \t \n \r
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimAll(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return input.Trim('\u0020', '\u3000', '\t', '\n', '\r');
        }
        
        public static string ToHex(this string input)
        {
            return ToHex(input, null, Encoding.UTF8);
        }

        public static string ToHex(this string input, string separator, Encoding encoding)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            
            if ((input.Length % 2) != 0)
            {
                input += "\u0020";//blank space
            }
            
            byte[] bytes = encoding.GetBytes(input);
            StringBuilder builder = new StringBuilder(bytes.Length);
            
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.AppendFormat("{0:X}", bytes[i]);
                if (! string.IsNullOrEmpty(separator) && (i != bytes.Length - 1))
                {
                    builder.Append(",");
                }
            }

            return builder.ToString();
        }

        public static string UnHex(this string hex)
        {
            return UnHex(hex, Encoding.UTF8);
        }

        public static string UnHex(this string hex, Encoding encoding)
        {
            if (hex == null)
            {
                throw new ArgumentNullException("hex");
            }
            hex = Clean(hex);
            hex = hex.Replace(",", string.Empty);
            hex = hex.Replace("\\", "");

            if (hex.Length % 2 != 0)
            {
                hex += "20";//blank space
            }
            
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }

            return encoding.GetString(bytes);
        }
    }
}
