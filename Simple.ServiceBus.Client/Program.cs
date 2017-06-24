using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Simple.ServiceBus.Common;
using Simple.ServiceBus.Common.Impl;

namespace Simple.ServiceBus.Client
{
    class Program
    {
        static PublishClient client = new PublishClient();

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
            SubscribeClient client = new SubscribeClient();
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
                            Publish(new Message<Test1Command>(new Test1Command()) { Header = header });
                            continue;
                        }

                        //if (i % 3 == 0)
                        {
                            Handle((new Message<Test2Command>(new Test2Command()) { Header = header }));
                            //continue;
                        }

                        //Publish(new Message() { Header = header, Body = i.ToString() + ">>" + Guid.NewGuid().ToString() });
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


        static void Handle<T>(Message<T> msg) where T : class, ICommand
        {
            Message dto = null;
            
            {
                dto = new Message() { Header = msg.Header, Body = msg.Body, TypeName = msg.TypeName };
            }
            Message result = null;
            using (ChannelFactory<IPublishService> factory = client.CreateChannelFactory())
            {
                factory.Open();

                result = factory.CreateChannel().PublishSync(dto);
            }

            if (result == null)
            {
                return;
            }

            Console.WriteLine(result.Body.ToString());
        }

        static void Publish(Message msg)
        {
            using (ChannelFactory<IPublishService> factory = client.CreateChannelFactory())
            {
                factory.Open();

                factory.CreateChannel().Publish(msg);
            }
        }

        static void Publish<T>(Message<T> msg) where T : class, ICommand
        {
            Message dto = new Message() { Header = msg.Header, Body = msg.Body, TypeName = msg.TypeName };
            //if (msg.Body is ICommand)
            //{
            //    dto = new Message() { Header = msg.Header, Body = msg.Body, TypeName= msg.TypeName };
            //}
            //else
            //{
            //    dto = msg;
            //}

            Publish(dto);
        }
    }
}
