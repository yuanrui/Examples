using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Simple.ServiceBus.Common.Impl
{
    public class PublishingClient
    {
        public IPublishing CreateProxy()
        {
            return CreateChannelFactory().CreateChannel();
        }

        public ChannelFactory<IPublishing> CreateChannelFactory()
        { 
            EndpointAddress endpointAddress = new EndpointAddress(ServiceSetting.PubAddress);

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);

            return new ChannelFactory<IPublishing>(tcpBinding, endpointAddress);
        }
    }
}
