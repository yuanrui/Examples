using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Simple.Common.Utility
{
    public static class DateTimeUtils
    {
        [DllImport("kernel32.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;

            public SYSTEMTIME(DateTime dt)
            {
                Year = (ushort)dt.Year;
                Month = (ushort)dt.Month;
                DayOfWeek = (ushort)dt.DayOfWeek;
                Day = (ushort)dt.Day;
                Hour = (ushort)dt.Hour;
                Minute = (ushort)dt.Minute;
                Second = (ushort)dt.Second;
                Milliseconds = (ushort)dt.Millisecond;
            }

            public DateTime ToDateTime()
            {
                return new DateTime(Year, Month, Day, Hour, Minute, Second, Milliseconds, DateTimeKind.Utc);
            }
        }

        public static DateTime GetSystemTime()
        {
            SYSTEMTIME stime = new SYSTEMTIME();
            GetSystemTime(ref stime);

            return stime.ToDateTime().ToLocalTime();
        }

        public static void SetSystemTime(DateTime time)
        {
            SYSTEMTIME systime = new SYSTEMTIME(time.ToUniversalTime());
            SetSystemTime(ref systime);
        }
    }
}
