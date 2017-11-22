using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Simple.ServiceBus.Client;
using Simple.ServiceBus.Messages;

namespace _Local.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Console.ReadLine();

            if (input == "s")
            {
                SubscribeClient client = new SubscribeClient();
                client.Register(new CallHandler());

                client.Subscribe("call");
            }
            else
            {
                PublishClient client = new PublishClient();
                
                do
                {
                    var inputMsg = new Message<CallCommand>(new CallCommand(), new MessageHeader() { RequestKey = "call" });
                    var result = client.Send<CallCommand, Call2Command>(inputMsg);
                    Console.WriteLine(result.Body);
                    Thread.Sleep(1000);
                    input = Console.ReadLine();
                } while (input != "q");
            }

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }

    public class CallCommand : ICommand
    {
        public string Id { get; set; }

        public DateTime Time { get; set; }

        public CallCommand()
        {
            Id = Guid.NewGuid().ToString();
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return Id + " " + Time.ToString();
        }
    }

    public class Call2Command : ICommand
    {
        public string Id { get; set; }

        public DateTime Time { get; set; }

        public Call2Command()
        {
            Id = Guid.NewGuid().ToString();
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            return Id + " " + Time.ToString();
        }
    }

    public class CallHandler : ICommandHandler<CallCommand, Call2Command>
    {

        public Call2Command Handle(CallCommand message)
        {
            return new Call2Command() { Id = "abc", Time = message.Time };
        }
    }
}
