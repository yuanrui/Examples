using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Simple.Common.Logging
{
    public class TraceHelper
    {
        public static void Write(string message)
        {
            Trace.Write(message);
        }

        public static void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public static void WriteLine(string message)
        {
            Trace.WriteLine(message);
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public static void WriteLineWithCategory(string message, string category)
        {
            Trace.WriteLine(message, category);
        }
    }
}
