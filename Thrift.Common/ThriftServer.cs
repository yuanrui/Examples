using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Thrift.Configuration;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Thrift
{
    public class ThriftServer
    {
        const String SECTION_NAME = "thrift.hosts";
        private static HostSetion hostConfiguration = null;
        private static Dictionary<String, Assembly> assemblyCache = new Dictionary<String, Assembly>();

        private TMultiplexedProcessor _multiplexedProcessor = new TMultiplexedProcessor();
        private TThreadPoolServer _threadPoolServer = null;

        protected static HostSetion Configuration 
        {
            get
            {
                if (hostConfiguration != null)
                {
                    return hostConfiguration;
                }
                
                hostConfiguration = ConfigurationManager.GetSection(SECTION_NAME) as HostSetion;

                return hostConfiguration;
            }
        }

        public void Start()
        {
            this.Start(Configuration.DefaultHost);
        }

        public void Start(String hostName)
        {
            var host = Configuration.Hosts[hostName];
            var serverTransport = new TServerSocket(host.Port, host.ClientTimeout, host.UseBufferedSockets);

            foreach (var service in host.Services)
            {
                _multiplexedProcessor.RegisterProcessor(service.ContractTypeName, GetProcessor(service.ContractTypeName, service.ContractAssemblyName, service.HandlerTypeName, service.HandlerAssemblyName));
            }

            _threadPoolServer = new TThreadPoolServer(_multiplexedProcessor, serverTransport, info => {
                Trace.WriteLine(info);
            });

            _threadPoolServer.Serve();
        }

        public void Stop()
        {
            if (_threadPoolServer == null)
            {
                return;
            }

            _threadPoolServer.Stop();
        }

        private Assembly GetAssembly(String assemblyName)
        {
            if (! assemblyCache.ContainsKey(assemblyName))
	        {
                assemblyCache[assemblyName] = Assembly.Load(assemblyName);
	        }
            
            return assemblyCache[assemblyName];
        }

        public TProcessor GetProcessor(String contractTypeName, String contractAssemblyName, String implTypeName, String implAssemblyName)
        {
            const String Processor = "+Processor";
            var contractType = GetAssembly(contractAssemblyName).GetType(contractTypeName + Processor);
            var handlerType = GetAssembly(implAssemblyName).GetType(implTypeName);
            
            var handlerInstance = Activator.CreateInstance(handlerType);
            var contractInstance = Activator.CreateInstance(contractType, new Object[] { handlerInstance }) as TProcessor;

            return contractInstance;
        }
    }
}
