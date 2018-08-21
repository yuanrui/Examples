using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift.Demo.Shared;

namespace Thrift.Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var transport = new TSocket("localhost", 6011);
            var protocol = new TBinaryProtocol(transport);
            var client = new AddressRpc.Client(protocol);

            transport.Open();
            var input = string.Empty;
            do
            {
                var id = client.GetNewId();
                var ent = client.Get(id);
                var updatedResult = client.Update(ent);

                Console.WriteLine("id:{0}\n obj:{1}\n update:{2}", id, ent.ToString(), updatedResult);
                input = Console.ReadLine();
            } while (input.ToLower() != "q");
        }
    }
}
