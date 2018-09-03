using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Banana.RPC;
using Thrift.Configuration;
using Thrift.Demo.Shared;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift.Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var section = ConfigurationManager.GetSection("thrift.clients") as ClientSection;
            Console.WriteLine("{0} {1} {2}", section.Host, section.Port, section.Timeout);
            foreach (var clientConfig in section.Clients)
            {
                Console.WriteLine("{0} {1} {2} {3}", clientConfig.ServiceName, clientConfig.Host, clientConfig.Port, clientConfig.Timeout);
            }

            var transport = new TSocket("localhost", 20188);
            var protocol = new TBinaryProtocol(transport);

            TMultiplexedProtocol protocol1 = new TMultiplexedProtocol(protocol, typeof(AddressRpc).FullName);
            var client = new AddressRpc.Client(protocol1);

            TMultiplexedProtocol protocol2 = new TMultiplexedProtocol(protocol, typeof(SmsSendShortMessageRpc).FullName);
            var client2 = new SmsSendShortMessageRpc.Client(protocol2);
            var factory = new ThriftClientFactory<SmsSendShortMessageRpc.Client>();
            client2 = factory.Create();
            var client3 = factory.Create();
            factory.Open();
            
            transport.Open();
            var input = string.Empty;
            do
            {
                try
                {
                    var id = client.GetNewId();
                    var ent = client.Get(id);
                    var updatedResult = client.Update(ent);
                    
                    Console.WriteLine("AddressRpc id:{0}\n obj:{1}\n update:{2}", id, ent.ToString(), updatedResult);
                    
                    var id2 = client2.GetNewId();
                    var id3 = client3.GetNewId();
                    var ent2 = client2.Get(id2.ToString());
                    var ent3 = client3.Get(id3.ToString());

                    Console.WriteLine("SmsSendShortMessageRpc id:{0} ent:{1} {2} {3}", id2, ent2, id3, ent3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                input = Console.ReadLine();
            } while (input.ToLower() != "q");
        }
    }
}
