using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simple.ServiceBus.Logging;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Client
{
    class Program
    {
        static PublishClient client = new PublishClient();

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new BusDefaultTraceListener());
            
            DoTest(args);
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }

        static void DoTest(string[] args)
        {
            var input = args.Length > 0 ? args[0] ?? Console.ReadLine() : Console.ReadLine();
            var key = "abc";
            var subKey = "s";
            if (input.Contains("="))
            {
                var inputs = input.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (inputs.Length > 1)
                {
                    subKey = inputs[0];
                    key = inputs[1];
                }
            }

            if (string.Equals(subKey, "s", StringComparison.OrdinalIgnoreCase))
            {
                Console.Title = "SubKey=" + key + " Start:" + DateTime.Now.ToString("yyyyMMddHHmmss");
                SubTest(key);
            }
            else
            {
                Console.Title = "PubKey=" + key + " Start:" + DateTime.Now.ToString("yyyyMMddHHmmss");
                PubTest(key);
            }
        }

        static void SubTest(string key)
        {
            for (int i = 0; i < 5; i++)
            {
                SubscribeClient client = new SubscribeClient();
                client.Register(new Test1Handler());
                client.Register(new Test3Handler());

                client.Subscribe(key);

                Thread.Sleep(1000);
            }
        }

        static void PubTest(string key)
        {
            var input = string.Empty;
            
            do
            {
                Console.WriteLine(DateTime.Now + ">>");
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var header = new MessageHeader { RequestKey = key, MessageKey = i.ToString() };

                        header.MessageKey = i.ToString();
                        
                        header.RouteType = RouteType.Single;

                        if (i % 2 == 0)
                        {
                            header.RouteType = RouteType.All;
                            SendAsyncTest3(new Test3InCommand() { Index = i }, header, i);
                            continue;
                        }
                        //if (i % 2 == 0)
                        {
                            //Publish(new Message<Test1Command>(new Test1Command()) { Header = header });
                            //continue;
                        }

                        SendTest3(new Test3InCommand() { Index = i }, header, i);                        
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(DateTime.Now + ">>" + ex.Message);
                }

                Console.WriteLine(DateTime.Now + ">> send finish");
                input = Console.ReadLine();

            } while (input != "q");
        }

        static void SendTest3(Test3InCommand cmd, MessageHeader header, Int64 index)
        { 
            var msg = new Message<Test3InCommand>(cmd, header);
            var result = client.Send<Test3InCommand, Test3OutCommand>(msg);
            Trace.WriteLine(index.ToString().PadLeft(3, '0') + "_" + DateTime.Now.ToString("HH:mm:ss") + ">> " + result.Body.ToString());
        }

        static void SendAsyncTest3(Test3InCommand cmd, MessageHeader header, Int64 index)
        {
            var msg = new Message<Test3InCommand>(cmd, header);
            client.SendAsync(msg);            
        }

    }
}
