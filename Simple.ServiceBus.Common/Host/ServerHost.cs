using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Simple.ServiceBus.Configuration;

namespace Simple.ServiceBus.Host
{
    public class ServerHost
    {
        ServiceHost _publishServiceHost = null;
        ServiceHost _subscribeServiceHost = null;

        public ServerHost()
        {
            _publishServiceHost = new ServiceHost(typeof(PublishService));
            _subscribeServiceHost = new ServiceHost(typeof(SubscribeService));
            var binding = NetSetting.GetBinding();

            _publishServiceHost.AddServiceEndpoint(typeof(IPublishService), binding, NetSetting.PubAddress);
            
            _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscribeService), binding, NetSetting.SubAddress);
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
