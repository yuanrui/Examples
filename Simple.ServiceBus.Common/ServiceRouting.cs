using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Simple.ServiceBus.Common
{
    public class ServiceRouting
    {
        public static ServiceRouting GlobalRouting = new ServiceRouting();

        public List<IPublishService> GetHandlers(string requestKey, RouteType routeType)
        {
            var handlers = ServiceRegister.GlobalRegister.GetHandler(requestKey);

            var result = new List<IPublishService>();

            if (routeType == RouteType.All)
            {
                foreach (var sub in handlers)
                {
                    sub.Value ++;
                    result.Add(sub.Key);
                }

                return result;
            }
            else
            {
                var sub = handlers.FirstOrDefault(m => m.Value == handlers.Min(t => t.Value));

                if (sub != null)
                {
                    sub.Value ++;

                    result.Add(sub.Key);
                }

                return result;
            }
        }

        public void UnRegister(string requestKey, IPublishService subscriber)
        {
            ServiceRegister.GlobalRegister.UnRegister(requestKey, subscriber);
        }
    }
}
