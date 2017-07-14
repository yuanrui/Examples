using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Simple.ServiceBus.Common.Inspect;

namespace Simple.ServiceBus.Common.Impl
{
    public class PublishClient
    {
        protected ChannelFactory<IPublishService> Factory { get; private set; }

        public PublishClient()
        {
            Factory = CreateChannelFactory();
        }

        protected ChannelFactory<IPublishService> CreateChannelFactory()
        {
            EndpointAddress endpointAddress = new EndpointAddress(NetSetting.PubAddress);
            
            var binding = NetSetting.GetBinding();
            
            var result = new ChannelFactory<IPublishService>(binding, endpointAddress);

            result.Endpoint.Behaviors.Add(new BusClientBehavior());

            return result;
        }

        public Message Send(Message message)
        {
            //Message result = null;

            //Service<IPublishService>.Use(m => 
            //{ 
            //    result = m.Publish(message); 
            //});

            //return result;

            Message result = null;
            var proxy = Factory.CreateChannel();
            bool success = false;

            try
            {
                result = proxy.Publish(message);

                ((IClientChannel)proxy).Close();
                success = true;
            }
            finally
            {
                if (! success)
                {
                    ((IClientChannel)proxy).Abort();
                }
            }

            return result;
        }

        public Message Send<T>(Message<T> message) where T : class, ICommand
        {
            Message dto = new Message() { Header = message.Header, Body = message.Body, TypeName = message.TypeName };

            return Send(dto);
        }
        
        public delegate void UseServiceDelegate<T>(T proxy);

        public static class Service<T>
        {
            static EndpointAddress endpointAddress = new EndpointAddress(NetSetting.PubAddress);

            //static NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None)
            //{
            //    MaxReceivedMessageSize = Int32.MaxValue,
            //    MaxBufferSize = Int32.MaxValue,
            //    MaxBufferPoolSize = 67108864,
            //    SendTimeout = TimeSpan.FromMinutes(1),
            //    ReceiveTimeout = TimeSpan.FromMinutes(1)
            //};

            public static ChannelFactory<T> _channelFactory = new ChannelFactory<T>(NetSetting.GetBinding(), endpointAddress);

            public static void Use(UseServiceDelegate<T> codeBlock)
            {
                IClientChannel proxy = (IClientChannel)_channelFactory.CreateChannel();
                bool success = false;
                try
                {
                    codeBlock((T)proxy);
                    proxy.Close();
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        proxy.Abort();
                    }
                }
            }
        }
    }
}
