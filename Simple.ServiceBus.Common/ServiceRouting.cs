using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
                    sub.Value.Up();
                    result.Add(sub.Key);
                }

                return result;
            }
            else
            {
                var sub = handlers.Count > 1 ? 
                    handlers.OrderBy(m => m.Value.Count).ThenBy(m => m.Value.Time).FirstOrDefault() 
                    : handlers.FirstOrDefault();

                if (sub != null)
                {
                    //Trace.Indent();
                    //Trace.Write("SubscriberId:" + sub.Value.Id + Environment.NewLine);
                    //Trace.Unindent();

                    sub.Value.Up();
                    
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
