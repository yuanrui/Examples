using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Thrift.Protocol
{
    public static class DateTimeExtension
    {
        private static readonly DateTime _startTime = new DateTime(1970, 1, 1);

        private static Int64 ToInt64(DateTime time)
        {
            return (time.Ticks - _startTime.Ticks) / 10000;
        }

        private static DateTime FromInt64(Int64 timestamp)
        {
            return _startTime.AddMilliseconds(timestamp);
        }

        public static void Read(this DateTime time, TProtocol iprot)
        {
            var value = iprot.ReadI64();
            //value object is referenced, not work.
            time = _startTime.AddMilliseconds(value);
        }

        public static void Write(this DateTime time, TProtocol iprot)
        {
            iprot.WriteI64(ToInt64(time));
        }
    }
}
