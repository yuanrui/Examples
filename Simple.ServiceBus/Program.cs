using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.ServiceBus.Core;
using Simple.ServiceBus.Handlers;

namespace Simple.ServiceBus
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestKey = "ReceiveService.DoReceive";
            var receiveService = new ReceiveService();
            receiveService.Register();
            var input = string.Empty;
            do
            {
                input = Console.ReadLine();
                var result = MessageBus.Send<string>(new Message { Body = input, Header = new MessageHeader { RequestKey = requestKey, MessageKey = Guid.NewGuid().ToString() } });

                Console.WriteLine(result);
            } while (input != "q");
            
            Console.ReadLine();
        }
    }
}
