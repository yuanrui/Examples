using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public class ServiceRouting
    {
        public static ServiceRouting GlobalRouting = new ServiceRouting();

        public List<IPublishService> GetHandlers(string requestKey)
        {
            return ServiceRegister.GlobalRegister.GetHandler(requestKey);
        }

        public void UnRegister(string requestKey, IPublishService subscriber)
        {
            ServiceRegister.GlobalRegister.UnRegister(requestKey, subscriber);
        }
    }
}
