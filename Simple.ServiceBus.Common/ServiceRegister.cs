using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Simple.ServiceBus.Common
{
    public class ServiceRegister : ConcurrentDictionary<string, List<IPublishService>>
    {
        public static ServiceRegister GlobalRegister = new ServiceRegister();
        
        public void Register(string requestKey, IPublishService subscriber)
        {
            if (this.ContainsKey(requestKey))
            {
                if (! this[requestKey].Contains(subscriber))
                {
                    this[requestKey].Add(subscriber);
                }
            }
            else
            {
                var list = new List<IPublishService>();
                list.Add(subscriber);
                this[requestKey] = list;
            }
        }

        public void UnRegister(string requestKey, IPublishService subscriber)
        {
            if (! this.ContainsKey(requestKey))
            {
                return;
            }

            if (this[requestKey].Contains(subscriber))
            {
                this[requestKey].Remove(subscriber);
            }
        }

        public List<IPublishService> GetHandler(string requestKey)
        {
            if (this.ContainsKey(requestKey))
            {
                return this[requestKey];
            }

            return null;
        }
    }
}
