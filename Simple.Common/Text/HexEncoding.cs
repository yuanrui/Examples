namespace Simple.Common.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HexEncoding
    {
        static char[] Digit = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static Byte[] GetBytes(String input)
        {
            if (input.Length % 2 == 1)
            {
                throw new ArgumentOutOfRangeException("input", "Must be an even number of hex digits");
            }

            var output = new byte[input.Length / 2];
            var textIndex = 0;
            for (var outputIndex = 0; outputIndex < output.Length; outputIndex++)
            {
                var b = (byte)((Hex(input[textIndex++]) << 4) + Hex(input[textIndex++]));
                output[outputIndex] = b;
            }

            return output;
        }

        public static String GetString(Byte[] bytes)
        {
            var output = new char[bytes.Length * 2];
            var outputIndex = 0;

            for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                var hex = bytes[byteIndex].ToString("X2");
                output[outputIndex++] = hex[0];
                output[outputIndex++] = hex[1];
            }
            
            return new string(output);
        }

        public static String GetString(Byte[] bytes, String separator)
        {
            var builder = new StringBuilder(bytes.Length * 3 - 1);

            for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                var hex = bytes[byteIndex].ToString("X2");
                builder.Append(hex);
                if (byteIndex + 1 < bytes.Length)
                {
                    builder.Append(separator);
                }
            }

            return builder.ToString();
        }

        public static string GetString(Byte ib)
        {
            char[] ob = new char[2];
            ob[0] = Digit[(ib >> 4) & 0X0F];
            ob[1] = Digit[ib & 0X0F];

            return new String(ob);
        }

        private static int Hex(char a)
        {
            if (a >= '0' && a <= '9')
            {
                return a - '0';
            }

            if (a >= 'a' && a <= 'f')
            {
                return a - 'a' + 10;
            }

            if (a >= 'A' && a <= 'F')
            {
                return a - 'A' + 10;
            }

            throw new ArgumentOutOfRangeException("a", String.Format("Character {0} is not hexadecimal", a));
        }
    }
}
