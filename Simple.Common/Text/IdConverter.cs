namespace Simple.Common.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class IdConverter
    {
        public static UInt64 ToLong(String id)
        {
            var bytes = Convert.FromBase64String(id);
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static String ToString(UInt64 id)
        {
            var bytes = BitConverter.GetBytes(id);
            Array.Reverse(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
