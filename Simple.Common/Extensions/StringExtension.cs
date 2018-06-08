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
        public static String Clean(this String input)
        {
            if (input == null)
            {
                return String.Empty;
            }

            return input.Replace("\u0020", String.Empty)
                .Replace("\t", String.Empty)
                .Replace("\n", String.Empty)
                .Replace("\r", String.Empty)
                .Replace("\u3000", String.Empty);
        }
        
        /// <summary>
        /// trim input string blank space \t \n \r
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String TrimAll(this String input)
        {
            if (input == null)
            {
                return String.Empty;
            }

            return input.Trim('\u0020', '\u3000', '\t', '\n', '\r');
        }
        
        public static String ToHex(this String input)
        {
            return ToHex(input, null, Encoding.UTF8);
        }

        public static String ToHex(this String input, String separator, Encoding encoding)
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
                if (! String.IsNullOrEmpty(separator) && (i != bytes.Length - 1))
                {
                    builder.Append(",");
                }
            }

            return builder.ToString();
        }

        public static String UnHex(this String hex)
        {
            return UnHex(hex, Encoding.UTF8);
        }

        public static String UnHex(this String hex, Encoding encoding)
        {
            if (hex == null)
            {
                throw new ArgumentNullException("hex");
            }
            hex = Clean(hex);
            hex = hex.Replace(",", String.Empty);
            hex = hex.Replace("\\", "");

            if (hex.Length % 2 != 0)
            {
                hex += "20";//blank space
            }

            Byte[] bytes = new Byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }

            return encoding.GetString(bytes);
        }

        public static Boolean IsAllWhiteSpace(this String value)
        {
            if (value == null || value.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (Char.IsWhiteSpace(value[i]) || value[i] == '\u3000')
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}
