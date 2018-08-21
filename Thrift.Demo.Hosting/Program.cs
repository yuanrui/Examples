using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Demo.Shared;

namespace Thrift.Demo.Hosting
{
    class Program
    {
        static void Main(string[] args)
        {
            TServerTransport transport = new TServerSocket(6011);
            //TProcessor processor = new AddressRpc.Processor(new AddressRpcImpl());
            TPrototypeProcessorFactory<AddressRpc.Processor, AddressRpcImpl> factory = new TPrototypeProcessorFactory<AddressRpc.Processor, AddressRpcImpl>();
            TProcessor processor = factory.GetProcessor(null);
            TSimpleServer server = new TSimpleServer(processor, transport, info => {
                Console.WriteLine(info);
            });

            Console.WriteLine("Begin service...");
            server.Serve();
        }
    }
}
