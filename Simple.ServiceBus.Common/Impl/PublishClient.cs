using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Simple.ServiceBus.Common.Impl
{
    public class PublishClient
    {
        public IPublishService CreateProxy()
        {
            return CreateChannelFactory().CreateChannel();
        }

        public ChannelFactory<IPublishService> CreateChannelFactory()
        { 
            EndpointAddress endpointAddress = new EndpointAddress(ServiceSetting.PubAddress);

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.MaxBufferPoolSize = 67108864;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);

            return new ChannelFactory<IPublishService>(tcpBinding, endpointAddress);
        }
    }
}
