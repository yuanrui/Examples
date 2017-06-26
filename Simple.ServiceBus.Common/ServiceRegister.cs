using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Simple.ServiceBus.Common
{
    public class ServiceRegister
    {
        private static ConcurrentDictionary<string, List<Pair<IPublishService, Int64>>> Cache = new ConcurrentDictionary<string, List<Pair<IPublishService, Int64>>>();

        public static ServiceRegister GlobalRegister = new ServiceRegister();
        
        public void Register(string requestKey, IPublishService subscriber)
        {
            if (Cache.ContainsKey(requestKey))
            {
                if (Cache[requestKey].All(m => m.Key != subscriber))
                {
                    Cache[requestKey].Add(new Pair<IPublishService, long>(subscriber, 0));
                }
            }
            else
            {
                var list = new List<Pair<IPublishService, long>>();
                list.Add(new Pair<IPublishService, long>(subscriber, 0));
                Cache.AddOrUpdate(requestKey, list, (k, l) => l);
            }
        }

        public void UnRegister(string requestKey, IPublishService subscriber)
        {
            if (! Cache.ContainsKey(requestKey))
            {
                return;
            }

            var item = Cache[requestKey].FirstOrDefault(m => m.Key == subscriber);

            if (item.Key == null)
            {
                return;
            }

            Cache[requestKey].Remove(item);
        }

        public List<Pair<IPublishService, long>> GetHandler(string requestKey)
        {
            if (Cache.ContainsKey(requestKey))
            {
                return Cache[requestKey];
            }

            return new List<Pair<IPublishService, long>>();
        }

        public class Pair<TKey, TValue>
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }

            public Pair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
