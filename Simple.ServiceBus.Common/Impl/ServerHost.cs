using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;

namespace Simple.ServiceBus.Common.Impl
{
    public class ServerHost
    {
        ServiceHost _publishServiceHost = null;
        ServiceHost _subscribeServiceHost = null;

        public ServerHost()
        {
            _publishServiceHost = new ServiceHost(typeof(PublishingService));
            _subscribeServiceHost = new ServiceHost(typeof(SubscriptionService));
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.SendTimeout = TimeSpan.FromSeconds(10);
            tcpBinding.ReceiveTimeout = TimeSpan.FromSeconds(10);
            
            _publishServiceHost.AddServiceEndpoint(typeof(IPublishing), tcpBinding,
                                ServiceSetting.PubAddress);
            
            _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), tcpBinding,
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
