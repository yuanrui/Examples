using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Simple.ServiceBus.Configuration;

namespace Simple.ServiceBus.Host
{
    public class ServerHost
    {
        ServiceHost _publishServiceHost = null;
        ServiceHost _subscribeServiceHost = null;
        Timer _timer;

        public ServerHost()
        {
            _publishServiceHost = new ServiceHost(typeof(PublishService));
            _subscribeServiceHost = new ServiceHost(typeof(SubscribeService));
            var binding = NetSetting.GetBinding();

            _publishServiceHost.AddServiceEndpoint(typeof(IPublishService), binding, NetSetting.PubAddress);
            
            _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscribeService), binding, NetSetting.SubAddress);

            _timer = new Timer(ShowStats, null, Timeout.Infinite, 5000);
        }

        public void Open()
        {
            _publishServiceHost.Open();
            _subscribeServiceHost.Open();

            _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));
        }

        public void Close()
        {
            _publishServiceHost.Close();
            _subscribeServiceHost.Close();
        }

        protected void ShowStats(object obj)
        {
            var list = ServiceRegister.GlobalRegister.GetStats();

            foreach (var item in list)
            {
                Trace.Write(item.Key + ":[" + string.Join(",", item.Value.Select(m => m.ToString())) + "]");
            }
        }
    }
}
