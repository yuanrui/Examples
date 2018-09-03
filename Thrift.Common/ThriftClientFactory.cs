using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Thrift.Configuration;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift
{
    public class ThriftClientFactory<TClient> : IDisposable
    {
        const String CLIENT = "+Client";
        const String SECTION_NAME = "thrift.clients";
        private static ClientSection clientConfiguration = null;
        private TProtocol _protocol;
        private TTransport _transport;
        private Type _clientType;

        protected static ClientSection Configuration
        {
            get
            {
                if (clientConfiguration != null)
                {
                    return clientConfiguration;
                }

                clientConfiguration = ConfigurationManager.GetSection(SECTION_NAME) as ClientSection;

                return clientConfiguration;
            }
        }

        public ThriftClientFactory()
        {
            _clientType = typeof(TClient);
            var serviceName = _clientType.FullName.Replace(CLIENT, String.Empty);
            _transport = new TSocket(Configuration.GetHost(serviceName), Configuration.GetPort(serviceName), Configuration.GetTimeout(serviceName));
            _protocol = new TMultiplexedProtocol(new TBinaryProtocol(_transport), serviceName);
        }

        public TClient Create()
        {
            var clientInstance = Activator.CreateInstance(_clientType, new Object[] { _protocol });

            return (TClient)clientInstance;
        }

        public void Open()
        {
            if (_transport == null)
            {
                return;
            }

            if (_transport.IsOpen)
            {
                return;
            }

            _transport.Open();
        }

        public void Close()
        {
            if (_transport == null)
            {
                return;
            }

            if (! _transport.IsOpen)
            {
                return;
            }

            _transport.Close();
        }

        public void Dispose()
        {
            if (_transport != null)
            {
                _transport.Dispose();
            }

            if (_protocol != null)
            {
                _protocol.Dispose();
            }            
        }
    }
}
