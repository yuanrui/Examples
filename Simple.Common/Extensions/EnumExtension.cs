using System;
using System.Collections.Generic;
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
    }
}
