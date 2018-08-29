using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Banana.RPC;
using Thrift.Demo.Shared;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Thrift.Demo.Hosting
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TMultiplexedProcessor multiplexedProcessor = new TMultiplexedProcessor();
                multiplexedProcessor.RegisterProcessor(typeof(AddressRpc).FullName, new AddressRpc.Processor(new AddressRpcImpl()));
                multiplexedProcessor.RegisterProcessor(typeof(SmsSendShortMessageRpc).FullName, new SmsSendShortMessageRpc.Processor(new SmsSendShortMessageRpcImpl()));

                TServerTransport transport = new TServerSocket(6011);
                TPrototypeProcessorFactory<AddressRpc.Processor, AddressRpcImpl> factory = new TPrototypeProcessorFactory<AddressRpc.Processor, AddressRpcImpl>();
                TProcessor processor = factory.GetProcessor(null);

                TSimpleServer server = new TSimpleServer(multiplexedProcessor, transport, info =>
                {
                    Console.WriteLine(info);
                });
                
                Console.WriteLine("Begin service...");

                server.Serve();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            Console.ReadLine();
        }
    }
}
