using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Simple.ServiceBus.Logging;

namespace Simple.ServiceBus.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            //Trace.Listeners.Add(new BusDefaultTraceListener());
            Trace.Listeners.Add(new FileLogTraceListener());

            if (Environment.UserInteractive)
            {
                StartApp();
            }
            else
            {
                StartWinService();
            }            
        }

        static void StartApp()
        {
            Console.Title = "Host Start:" + DateTime.Now.ToString("yyyyMMddHHmmss");
            Trace.WriteLine(Console.Title);
            ServerHost host = new ServerHost();
            host.Open();
            var input = string.Empty;

            do
            {
                Console.WriteLine("Press 'q' Key To Exit...");
                input = Console.ReadLine();

            } while (!string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));
        }

        private static void StartWinService()
        {
            using (WinService service = new WinService())
            {
                ServiceBase.Run(service);
            }
        }
    }
}
