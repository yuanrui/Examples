using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common.Impl
{
    public class ServerHost
    {
        ServiceHost _publishServiceHost = null;
        ServiceHost _subscribeServiceHost = null;

        public ServerHost()
        {
            _publishServiceHost = new ServiceHost(typeof(PublishService));
            _subscribeServiceHost = new ServiceHost(typeof(SubscribeService));
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.MaxBufferPoolSize = 67108864;

            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);
            
            _publishServiceHost.AddServiceEndpoint(typeof(IPublishService), tcpBinding,
                                ServiceSetting.PubAddress);
            
            _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscribeService), tcpBinding,
                                ServiceSetting.SubAddress);
        }

        public void Open()
        {
            _publishServiceHost.Open();
            _subscribeServiceHost.Open();
        }

        public void Close()
        {
            _publishServiceHost.Close();
            _subscribeServiceHost.Close();
        }
    }
}
