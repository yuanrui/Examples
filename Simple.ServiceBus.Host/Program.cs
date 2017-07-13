using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Simple.ServiceBus.Common.Impl;

namespace Simple.ServiceBus.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Console.Title = "Host Start:" + DateTime.Now.ToString("yyyyMMddHHmmss");
            ServerHost host = new ServerHost();
            host.Open();
            var input = string.Empty;
            
            Console.WriteLine("Press 'q' Key To Exit...");

            do
            {
                input = Console.ReadLine();
            } while (! string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));            
        }
    }
}
