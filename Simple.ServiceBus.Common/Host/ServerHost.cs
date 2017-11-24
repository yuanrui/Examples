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
        int _currentDay;

        public ServerHost()
        {
            _publishServiceHost = new ServiceHost(typeof(PublishService));
            _subscribeServiceHost = new ServiceHost(typeof(SubscribeService));
            var binding = NetSetting.GetBinding();

            _publishServiceHost.AddServiceEndpoint(typeof(IPublishService), binding, NetSetting.PubAddress);
            
            _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscribeService), binding, NetSetting.SubAddress);
            _currentDay = DateTime.Now.Day;

            _timer = new Timer(ShowStats, null, Timeout.Infinite, 5000);
        }

        public void Open()
        {
            _publishServiceHost.Open();
            _subscribeServiceHost.Open();

            _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));
        }

        public void Close()
        {
            _publishServiceHost.Close();
            _subscribeServiceHost.Close();
        }

        protected void ShowStats(object obj)
        {
            var list = ServiceRegister.GlobalRegister.GetStats();
            var compareTime = DateTime.Now.AddMinutes(-1);
            var disconnectedList = list.Where(m => m.Value.Any(t => t.Time < compareTime));
            foreach (var disConn in disconnectedList)
            {
                var disConnHandlers = ServiceRegister.GlobalRegister.GetHandler(disConn.Key).ToList();
                var tmpPair = list.FirstOrDefault(m => m.Key == disConn.Key);
                
                foreach (var item in disConnHandlers)
                {
                    if (disConn.Value.Any(m => m.Id == item.Value.Id && m.Time == item.Value.Time && m.Time < compareTime))
                    {
                        tmpPair.Value.RemoveAll(m => m.Id == item.Value.Id && m.Time == item.Value.Time);                        
                        
                        ServiceRegister.GlobalRegister.UnRegister(disConn.Key, item.Key);
                        Trace.WriteLine(item.Key + ":[" + item.Value.Id + " time out removed, last access time:" + item.Value.Time.ToString("yyyyMMddHHmmss") + "]");
                    }
                }
            }

            foreach (var item in list)
            {
                Trace.WriteLine(item.Key + ":[" + string.Join(",", item.Value.Select(m => m.ToString())) + "]");
            }

            TryResetStats();
        }

        protected void TryResetStats()
        {
            if (_currentDay == DateTime.Now.Day)
            {
                return;
            }

            Console.Clear();
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));

            GC.Collect();

            var list = ServiceRegister.GlobalRegister.GetStats();

            foreach (var item in list)
            {
                foreach (var runInfoItem in item.Value)
                {
                    runInfoItem.ResetCount();
                }
            }

            _currentDay = DateTime.Now.Day; 
        }
    }
}
