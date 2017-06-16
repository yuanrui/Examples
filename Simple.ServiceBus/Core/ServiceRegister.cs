using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Core
{
    public delegate void ServiceHandler(object parameter, ref object result);

    public class ServiceRegister : Dictionary<string, ServiceHandler>
    {
        public static ServiceRegister GlobalRegister = new ServiceRegister();

        public void Register(string requestKey, ServiceHandler handler)
        {
            this[requestKey] = handler;
        }

        public ServiceHandler GetHandler(string requestKey)
        {
            if (this.ContainsKey(requestKey))
            {
                return this[requestKey];
            }

            return null;
        }
    }
}
