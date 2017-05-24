using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class DateTimeExtension
    {
        private static readonly DateTime _startTime = new DateTime(1970, 1, 1);

        public static Int64 ToInt64(this DateTime time)
        {
            return (time.Ticks - _startTime.Ticks) / 10000;
        }

        /// <summary>
        /// to yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToLongDateTimeString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
