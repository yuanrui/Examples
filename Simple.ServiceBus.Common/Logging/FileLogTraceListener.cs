using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Logging
{
    public class FileLogTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            Console.WriteLine("{0}>>{1}", DateTime.Now.ToString("HH:mm:ss"), message);
        }

        public override void WriteLine(string message)
        {
            SimpleLogger.Info(message);
            Console.WriteLine("{0}>>{1}", DateTime.Now.ToString("HH:mm:ss"), message);
        }
    }
}
