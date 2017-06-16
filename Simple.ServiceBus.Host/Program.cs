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
            ServerHost host = new ServerHost();
            host.Open();
            
            SubscriptionClient client = new SubscriptionClient();
            client.Subscribe("abc");
            

            Console.ReadLine();
        }
    }
}
