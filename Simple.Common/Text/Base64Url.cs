namespace Simple.Common.Text
{
    using System;
    
    public static class Base64Url
    {
        public static string Encode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0];
            output = output.Replace('+', '-');
            output = output.Replace('/', '_');
            return output;
        }

        public static byte[] Decode(string input)
        {
            var output = input;
            output = output.Replace('-', '+');
            output = output.Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0: break;
                case 2: output += "=="; break;
                case 3: output += "="; break;
                default: throw new System.ArgumentOutOfRangeException("input", "Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output);
            return converted;
        }
    }
}
