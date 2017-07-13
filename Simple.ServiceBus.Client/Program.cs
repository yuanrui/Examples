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
            SubscribeClient client = new SubscribeClient();
            client.Subscribe(key);
        }

        static void PubTest(string key)
        {
            var input = string.Empty;
            var header = new Common.MessageHeader { RequestKey = key, MessageKey = Guid.NewGuid().ToString() };

            do
            {
                Console.WriteLine(DateTime.Now + ">>");
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        header.MessageKey = i.ToString();
                        
                        header.RouteType = RouteType.Single;

                        if (i % 2 == 0)
                        {
                            header.RouteType = RouteType.All;
                        }
                        //if (i % 2 == 0)
                        {
                            //Publish(new Message<Test1Command>(new Test1Command()) { Header = header });
                            //continue;
                        }

                        //if (i % 3 == 0)
                        {
                            Handle((new Message<Test1Command>(new Test1Command() { Index = i }) { Header = header }), i);
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


        static void Handle<T>(Message<T> msg, Int64 index) where T : class, ICommand
        {
            //Message dto = null;
            
            //{
            //    dto = new Message() { Header = msg.Header, Body = msg.Body, TypeName = msg.TypeName };
            //}
            //Message result = null;
            //using (ChannelFactory<IPublishService> factory = client.CreateChannelFactory())
            //{
            //    factory.Open();
            //    var channel = factory.CreateChannel();

            //    result = factory.CreateChannel().Publish(dto);
            //}

            var result = client.Send(msg);

            if (result == null)
            {
                return;
            }

            Console.WriteLine(index.ToString().PadLeft(3, '0') + "_" + DateTime.Now.ToString("HH:mm:ss") + ">> " + result.Body.ToString());
        }

        static void Publish(Message msg)
        {
            //using (ChannelFactory<IPublishService> factory = client.CreateChannelFactory())
            //{
            //    factory.Open();

            //    factory.CreateChannel().Publish(msg);
            //}

            client.Send(msg);
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
