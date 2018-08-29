using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift.Demo.Shared;
using Banana.RPC;

namespace Thrift.Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var transport = new TSocket("localhost", 6011);
            var protocol = new TBinaryProtocol(transport);

            TMultiplexedProtocol protocol1 = new TMultiplexedProtocol(protocol, typeof(AddressRpc).FullName);
            var client = new AddressRpc.Client(protocol1);

            TMultiplexedProtocol protocol2 = new TMultiplexedProtocol(protocol, typeof(SmsSendShortMessageRpc).FullName);
            var client2 = new SmsSendShortMessageRpc.Client(protocol2);

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
                    var ent2 = client2.Get(id2.ToString());
                    Console.WriteLine("SmsSendShortMessageRpc id:{0} ent:{1}", id2, ent2);
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
