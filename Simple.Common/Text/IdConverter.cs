namespace Simple.Common.Text
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        public static Guid ToGuid(UInt16 id)
        {
            var input = id.ToString("X4");
            if (input.Length == 4)
            {
                //Bluetooth UUID
                input = "0000" + input + "-0000-1000-8000-00805F9B34FB";
            }
            return Guid.ParseExact(input, "D");
        }

        public static UInt16 ToUInt16(Guid uuid)
        {
            UInt16 result = 0;
            var id = uuid.ToString();
            if (id.Length > 8)
            {
                id = id.Substring(4, 4);

                UInt16.TryParse(id, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out result);
            }

            return result;
        }

        public static String ToUInt16Hex(Guid uuid)
        {
            var id = ToUInt16(uuid);

            return "0x" + id.ToString("X4");
        }
    }
}
