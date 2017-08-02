using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Simple.ServiceBus.Logging;

namespace Simple.ServiceBus.Host
{
    class Program
    {
        static void Main(string[] args)
        {
//#if DEBUG
//            Trace.Listeners.Add(new BusDefaultTraceListener());
//#else
            Trace.Listeners.Add(new FileLogTraceListener());
//#endif
            Console.Title = "Host Start:" + DateTime.Now.ToString("yyyyMMddHHmmss");
            Trace.WriteLine(Console.Title);
            ServerHost host = new ServerHost();
            host.Open();
            var input = string.Empty;
            
            do
            {
                Console.WriteLine("Press 'q' Key To Exit...");
                input = Console.ReadLine();
                
            } while (! string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));            
        }
    }
}
