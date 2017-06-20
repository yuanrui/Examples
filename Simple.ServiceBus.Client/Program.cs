using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Simple.ServiceBus.Common;
using Simple.ServiceBus.Common.Impl;

namespace Simple.ServiceBus.Client
{
    class Program
    {
        static PublishingClient client = new PublishingClient();

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            
            DoTest(args);
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }

        static void DoTest(string[] args)
        {
            var input = args.Length > 0 ? args[0] ?? Console.ReadLine() : Console.ReadLine();

            if (string.Equals(input, "s", StringComparison.OrdinalIgnoreCase))
            {
                SubTest();
            }
            else
            {
                PubTest();
            }
        }

        static void SubTest()
        {
            SubscriptionClient client = new SubscriptionClient();
            client.Subscribe("abc");
        }

        static void PubTest()
        {
            var pub = client.CreateProxy();
            var input = string.Empty;
            var header = new Common.MessageHeader { RequestKey = "abc", MessageKey = Guid.NewGuid().ToString() };

            do
            {
                Console.WriteLine(DateTime.Now + ">>");
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        header.MessageKey = i.ToString();

                        //if (i % 2 == 0)
                        {
                            Publish(new Message<Test1>(new Test1()) { Header = header });
                            //pub.Publish((new Message<Test1>(new Test1()) { Header = header }).ToMessage());
                            continue;
                        }

                        if (i % 3 == 0)
                        {
                            //pub.Publish2((new Message<Test2>(new Test2()) { Header = header }));
                            Publish((new Message<Test2>(new Test2()) { Header = header }));
                            continue;
                        }

                        Publish(new Message() { Header = header, Body = i.ToString() + ">>" + Guid.NewGuid().ToString() });
                        //pub.Publish(new Message() { Header = header, Body = i.ToString() + ">>" + Guid.NewGuid().ToString() });
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(DateTime.Now + ">>" + ex.Message);

                    try
                    {
                        Publish(new Message() { Header = header, Body = DateTime.Now.ToShortTimeString() + " >> " + Guid.NewGuid().ToString() });
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Pub_Ex:" + ex2.Message);
                    }
                }

                Console.WriteLine(DateTime.Now + ">> send finish");
                input = Console.ReadLine();

            } while (input != "q");
        }

        static void Publish(Message msg)
        {
            Message dto = null;
            if (msg.Body is IBusData)
            {
                dto = new Message() { Header = msg.Header, Body = msg.Body };
            }
            else
            {
                dto = msg;
            }

            using (ChannelFactory<IPublishing> factory = client.CreateChannelFactory())
            {
                factory.Open();
                
                factory.CreateChannel().Publish(dto);
            }
        }
    }
}
