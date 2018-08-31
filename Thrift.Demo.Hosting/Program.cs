using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Banana.RPC;
using Thrift.Configuration;
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
                var section = ConfigurationManager.GetSection("thrift.hosts") as HostSetion;
                foreach (var host in section.Hosts)
                {
                    Console.WriteLine("{0} {1} {2} {3} {4}", host.Name, host.Port, host.MinThreadPoolSize, host.MaxThreadPoolSize, host.Services.Count);
                    foreach (var service in host.Services)
                    {
                        Console.WriteLine("{0} {1}", service.Contract, service.Handler);
                    }
                }

                ThriftServer serverHost = new ThriftServer();
                serverHost.Start();
                

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
